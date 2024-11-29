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

public class DbBackupPlansController : BaseAdminITController
{
   private readonly IDbBackupPlanService _backupPlanService;
   private readonly IDatabaseService _databaseService;
   private readonly IMapper _mapper;
     
   public DbBackupPlansController(IDbBackupPlanService backupPlanService, IDatabaseService databaseService, IMapper mapper)
   {
      _backupPlanService = backupPlanService;
      _databaseService = databaseService;
      _mapper = mapper;
   }
   [HttpGet("init")]
   public async Task<ActionResult<DbBackupPlansIndexModel>> Init()
   {
      var request = new DbBackupPlansFetchRequest();
      
      var servers = await FetchDatabasesAsync();
      return new DbBackupPlansIndexModel(request, servers.MapViewModelList(_mapper));
   }
   async Task<ICollection<Database>> FetchDatabasesAsync()
   {
      var includes = new List<string>() { nameof(Database.Server) };
      var list = await _databaseService.FetchAsync(includes);

      if (list.HasItems()) list = list.GetOrdered();
      return list.ToList(); 
   }
   [HttpGet]
   public async Task<ActionResult<ICollection<DbBackupPlanViewModel>>> Index(int dbId)
   {
      var list = await _backupPlanService.FetchAsync(new Database() { Id = dbId });

      list = list.GetOrdered().ToList();
      return list.MapViewModelList(_mapper);
   }


   [HttpGet("create")]
   public ActionResult<DbBackupPlanAddRequest> Create() => new DbBackupPlanAddRequest(new DbBackupPlanAddForm() { Active = true });


   [HttpPost]
   public async Task<ActionResult<DbBackupPlanViewModel>> Store([FromBody] DbBackupPlanAddForm model)
   {
      await ValidateRequestAsync(model, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new DbBackupPlan();
      model.SetValuesTo(entity);
      if (model.Active) entity.Order = 0;
      else entity.Order = -1;

      entity = await _backupPlanService.CreateAsync(entity, User.Id());

      return Ok(entity.MapViewModel(_mapper));
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<DbBackupPlanEditRequest>> Edit(int id)
   {
      var entity = await _backupPlanService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var view = entity.MapViewModel(_mapper);

      var form = new DbBackupPlanEditForm();
      view.SetValuesTo(form);

      form.CanRemove = !entity.Active;

      return new DbBackupPlanEditRequest(form);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] DbBackupPlanEditForm model)
   {
      var entity = await _backupPlanService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);
      if (model.Active) entity.Order = 0;
      else entity.Order = -1;

      await _backupPlanService.UpdateAsync(entity, User.Id());

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _backupPlanService.GetByIdAsync(id);
      if (entity == null) return NotFound();
     
      await _backupPlanService.RemoveAsync(entity, User.Id());

      return NoContent();
   }

   async Task ValidateRequestAsync(DbBackupPlanBaseForm model, int id)
   {
      var labels = new DbBackupPlanLabels();

      var db = await _databaseService.GetByIdAsync(model.DatabaseId);
      if (db == null)
      {
         ModelState.AddModelError(nameof(model.DatabaseId), ValidationMessages.NotExist($"{labels.Database} id = {id}"));
      }
   }


}