using ApplicationCore.Services;
using ApplicationCore.Views.IT;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models.IT;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Infrastructure.Paging;
using ApplicationCore.Consts;
using Web.Models.IT;
using Infrastructure.Views;
using System.ComponentModel.DataAnnotations;

namespace Web.Controllers.Admin.IT;

public class ServersController : BaseAdminITController
{
   private readonly IServerService _serverService;
   private readonly IHostService _hostService;
   private readonly IMapper _mapper;
     
   public ServersController(IServerService serverService, IHostService databaseService, IMapper mapper)
   {
      _serverService = serverService;
      _hostService = databaseService;
      _mapper = mapper;
   }
   [HttpGet("init")]
   public async Task<ActionResult<ServersIndexModel>> Init()
   {
      string type = ServerType.Web;
      var request = new ServersFetchRequest(type);

      return new ServersIndexModel(request);
   }
   [HttpGet]
   public async Task<ActionResult<ICollection<ServerViewModel>>> Index(string type = "")
   {
      string include = nameof(Server.Host);
      var list = await _serverService.FetchAsync(include);

      if (!string.IsNullOrEmpty(type))
      {
         ValidateType(type);
         if (!ModelState.IsValid) return BadRequest(ModelState);

         list = list.Where(x => type.EqualTo(x.Type)).ToList();
      }
      

      //if(string.IsNullOrEmpty)

      if (list.HasItems())
      {
         list = list.GetOrdered().ToList();
      }
      return list.MapViewModelList(_mapper);
   }


   [HttpGet("create")]
   public async Task<ActionResult<ServersAddRequest>> Create()
   { 
      var form = new ServerAddForm() { Provider = DbProvider.SQLServer };
      var hosts = await _hostService.FetchAsync();
      return new ServersAddRequest(form, hosts.MapViewModelList(_mapper));
   }



   [HttpPost]
   public async Task<ActionResult<ServerViewModel>> Store([FromBody] ServerAddForm model)
   {
      await ValidateRequestAsync(model, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new Server();
      model.SetValuesTo(entity);
      entity.SetCreated(User.Id());

      entity = await _serverService.CreateAsync(entity, User.Id());

      return Ok(entity.MapViewModel(_mapper));
   }

   [HttpGet("{id}")]
   public async Task<ActionResult<ServerViewModel>> Details(int id)
   {
      string include = nameof(Server.Host);
      var entity = await _serverService.GetByIdAsync(id, include);
      if (entity == null) return NotFound();

      return entity.MapViewModel(_mapper);
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<ServersEditRequest>> Edit(int id)
   {
      var entity = await _serverService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var form = new ServerEditForm();
      entity.SetValuesTo(form);
      var hosts = await _hostService.FetchAsync();
      return new ServersEditRequest(form, hosts.MapViewModelList(_mapper));
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] ServerEditForm model)
   {
      var entity = await _serverService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);
      entity.SetUpdated(User.Id());

      await _serverService.UpdateAsync(entity, User.Id());

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _serverService.GetByIdAsync(id);
      if (entity == null) return NotFound();
     
      await _serverService.RemoveAsync(entity, User.Id());

      return NoContent();
   }

   async Task ValidateRequestAsync(ServerBaseForm model, int id)
   {
      var labels = new ServerLabels();
      ValidateType(model.Type);
      if (!ModelState.IsValid) return;

      var host = await _hostService.GetByIdAsync(model.HostId);
      if (host == null)
      {
         ModelState.AddModelError(nameof(model.HostId), ValidationMessages.NotExist($"{labels.Host} id = { id }"));
      }
   }

   void ValidateType(string type)
   {
      if (type.EqualTo(ServerType.Web))
      {

      }
      else if (type.EqualTo(ServerType.Db))
      {

      }
      else if (type.EqualTo(ServerType.Db))
      {

      }
      else
      {
         ModelState.AddModelError("type", ValidationMessages.NotExist(type));
      }
   }
}