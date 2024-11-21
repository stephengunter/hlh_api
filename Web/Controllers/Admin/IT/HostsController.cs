using ApplicationCore.Services;
using ApplicationCore.Views.IT;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Infrastructure.Paging;
using ApplicationCore.Consts;
using Web.Models.IT;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Controllers.Admin.IT;

public class HostsController : BaseAdminITController
{
   private readonly IHostService _hostService; 
   private readonly ICredentialInfoService _credentialInfoService;
   private readonly IMapper _mapper;
     
   public HostsController(IHostService databaseService, ICredentialInfoService credentialInfoService, IMapper mapper)
   {
      _hostService = databaseService; 
      _credentialInfoService = credentialInfoService;
      _mapper = mapper;
   }
   [HttpGet("init")]
   public async Task<ActionResult<HostsIndexModel>> Init()
   {
      int page = 1;
      int pageSize = 10;
      var request = new HostsFetchRequest(page, pageSize);
      var providers = new List<string>() { DbProvider.SQLServer, DbProvider.PostgreSql };

      return new HostsIndexModel(request, providers);
   }
   [HttpGet]
   public async Task<ActionResult<PagedList<ApplicationCore.Models.IT.Host, HostViewModel>>> Index(bool active = true, int page = 1, int pageSize = 10)
   {
      var list = await _hostService.FetchAsync();

      if (list.HasItems())
      {
         list = list.Where(x => x.Active == active);

         list = list.GetOrdered().ToList();
      }
      return list.GetPagedList(_mapper, page, pageSize);
   }


   [HttpGet("create")]
   public ActionResult<HostAddForm> Create() => new HostAddForm();


   [HttpPost]
   public async Task<ActionResult<HostViewModel>> Store([FromBody] HostAddForm model)
   {
      await ValidateRequestAsync(model, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new ApplicationCore.Models.IT.Host();
      model.SetValuesTo(entity);

      entity = await _hostService.CreateAsync(entity, User.Id());

      return Ok(entity.MapViewModel(_mapper));
   }

   [HttpGet("{id}")]
   public async Task<ActionResult<HostViewModel>> Details(int id)
   {
      var entity = await _hostService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var credentialInfoes = await _credentialInfoService.FetchAsync(entity);
      entity.LoadCredentialInfoes(credentialInfoes);
      return entity.MapViewModel(_mapper);
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<HostEditForm>> Edit(int id)
   {
      var entity = await _hostService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var model = new HostEditForm();
      entity.SetValuesTo(model);

      return model;
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] HostEditForm model)
   {
      var entity = await _hostService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);

      await _hostService.UpdateAsync(entity, User.Id());

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _hostService.GetByIdAsync(id);
      if (entity == null) return NotFound();
     
      await _hostService.RemoveAsync(entity, User.Id());

      return NoContent();
   }

   async Task ValidateRequestAsync(BaseHostForm model, int id)
   {
      var labels = new HostLabels();
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError(nameof(model.Title), ValidationMessages.Required(labels.Title));
      if (String.IsNullOrEmpty(model.Key)) ModelState.AddModelError(nameof(model.Key), ValidationMessages.Required(labels.Key));
      if (String.IsNullOrEmpty(model.IP)) ModelState.AddModelError(nameof(model.IP), ValidationMessages.Required(labels.IP));
      if (!model.IP.IsValidIpAddress()) ModelState.AddModelError(nameof(model.IP), ValidationMessages.WrongFormatOf(labels.IP));

      if (!ModelState.IsValid) return;

      var hosts = await _hostService.FetchAsync();
      var exist = hosts.FirstOrDefault(x => x.IP == model.IP);
      if (exist != null && exist.Id != id)
      {
         ModelState.AddModelError("ip", ValidationMessages.Duplicate(labels.IP));
      }
      exist = hosts.FirstOrDefault(x => x.Title == model.Title);
      if (exist != null && exist.Id != id)
      {
         ModelState.AddModelError("title", ValidationMessages.Duplicate(labels.Title));
      }
      exist = hosts.FirstOrDefault(x => x.Key == model.Key);
      if (exist != null && exist.Id != id)
      {
         ModelState.AddModelError("key", ValidationMessages.Duplicate(labels.Key));
      }
   }


}