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
using Microsoft.Build.Execution;
using System.Collections.Generic;

namespace Web.Controllers.Admin.IT;

public class DatabasesController : BaseAdminITController
{
   private readonly IDatabaseService _databaseService;
   private readonly IServerService _serverService;
   private readonly IMapper _mapper;
     
   public DatabasesController(IDatabaseService databaseService, IServerService serverService, IMapper mapper)
   {
      _databaseService = databaseService;
      _serverService = serverService;
      _mapper = mapper;
   }
   [HttpGet("init")]
   public async Task<ActionResult<DatabasesIndexModel>> Init()
   {
      var request = new DatabasesFetchRequest();

      var servers = await FetchServersAsync();
      return new DatabasesIndexModel(request, servers.MapViewModelList(_mapper));
   }
   async Task<ICollection<Server>> FetchServersAsync()
   {
      string type = ServerType.Db;
      string include = nameof(Server.Host);
      var list = await _serverService.FetchAsync(include);

      list = list.Where(x => type.EqualTo(x.Type)).ToList();
      if (list.HasItems()) list = list.GetOrdered();
      return list.ToList(); 
   }
   [HttpGet]
   public async Task<ActionResult<ICollection<DatabaseViewModel>>> Index(int? serverId)
   {
      string include = nameof(Database.Server);
      var list = await _databaseService.FetchAsync(include);
      if (serverId.HasValue && serverId.Value > 0) list = list.Where(x => x.ServerId == serverId.Value);

      list = list.GetOrdered().ToList();
      return list.MapViewModelList(_mapper);
   }


   [HttpGet("create")]
   public ActionResult<DatabaseAddForm> Create() => new DatabaseAddForm();


   [HttpPost]
   public async Task<ActionResult<DatabaseViewModel>> Store([FromBody] DatabaseAddForm model)
   {
      await ValidateRequestAsync(model, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new Database();
      model.SetValuesTo(entity);
      entity.SetCreated(User.Id());

      entity = await _databaseService.CreateAsync(entity, User.Id());

      return Ok(entity.MapViewModel(_mapper));
   }
   [HttpGet("{id}")]
   public async Task<ActionResult<DatabaseViewModel>> Details(int id)
   {
      string include = $"{nameof(Database.Server)},{nameof(Database.BackupPlans)}";
      var entity = await _databaseService.GetByIdAsync(id, include);
      if (entity == null) return NotFound();

      return entity.MapViewModel(_mapper);
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<DatabaseEditRequest>> Edit(int id)
   {
      var entity = await _databaseService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var form = new DatabaseEditForm();
      entity.SetValuesTo(form);
      var servers = await FetchServersAsync();
      return new DatabaseEditRequest(form, servers.MapViewModelList(_mapper));
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] DatabaseEditForm model)
   {
      var entity = await _databaseService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);
      entity.SetUpdated(User.Id());

      await _databaseService.UpdateAsync(entity, User.Id());

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _databaseService.GetByIdAsync(id);
      if (entity == null) return NotFound();
     
      await _databaseService.RemoveAsync(entity, User.Id());

      return NoContent();
   }

   async Task ValidateRequestAsync(DatabaseBaseForm model, int id)
   {
      var labels = new DatabaseLabels();

      var server = await _serverService.GetByIdAsync(model.ServerId);
      if (server == null)
      {
         ModelState.AddModelError(nameof(model.ServerId), ValidationMessages.NotExist($"{labels.Server} id = {id}"));
      }
   }


}