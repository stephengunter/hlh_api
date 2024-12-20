using ApplicationCore.Services;
using ApplicationCore.Views.IT;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models.IT;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Infrastructure.Paging;
using Web.Models;
using ApplicationCore.Consts;
using Microsoft.Build.Execution;
using Web.Models.IT;

namespace Web.Controllers.Admin.IT;

public class SystemAppsController : BaseAdminITController
{
   private readonly ISystemAppService _systemAppService; 
   private readonly IServerService _serverService;
   private readonly IMapper _mapper;
     
   public SystemAppsController(ISystemAppService systemAppService, IServerService serverService, IMapper mapper)
   {
      _systemAppService = systemAppService;
      _serverService = serverService;
      _mapper = mapper;
   }
   [HttpGet("init")]
   public async Task<ActionResult<SystemAppsIndexModel>> Init()
   {
      bool active = true;
      int centralized = -1;
      int page = 1;
      int pageSize = 25;

      string? keyword = string.Empty;

      var request = new SystemAppFetchRequest(active, centralized, page, pageSize, keyword);

      return new SystemAppsIndexModel(request);
   }

   [HttpGet]
   public async Task<ActionResult<PagedList<SystemApp, SystemAppViewModel>>> Index(bool active, int centralized, int page = 1, int pageSize = 10)
   {
      var list = await _systemAppService.FetchAsync();

      if (list.HasItems())
      {
         if (centralized == 0) list = list.Where(x => !x.Centralized);
         else if (centralized == 1) list = list.Where(x => x.Centralized);

         list = list.Where(x => x.Active == active);

         list = list.GetOrdered().ToList();
      }
      list.MapViewModelList(_mapper);
      return list.GetPagedList(_mapper, page, pageSize);
   }


   [HttpGet("create")]
   public async Task<ActionResult<SystemAppAddRequest>> Create()
   {
      var form = new SystemAppAddForm() { };
      
      var servers = await _serverService.FetchAsync(include: "Host");
      return new SystemAppAddRequest(form, servers.MapViewModelList(_mapper));
   }


   [HttpPost]
   public async Task<ActionResult<SystemAppViewModel>> Store([FromBody] SystemAppAddForm form)
   {
      await ValidateRequestAsync(form, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new SystemApp();
      form.SetValuesTo(entity);
      entity.SetCreated(User.Id());

      entity = await _systemAppService.CreateAsync(entity, User.Id());

      return entity.MapViewModel(_mapper);
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult<SystemAppEditRequest>> Edit(int id)
   {
      var entity = await _systemAppService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var form = new SystemAppEditForm();
      entity.SetValuesTo(form);
      var servers = await _serverService.FetchAsync();
      return new SystemAppEditRequest(form, servers.MapViewModelList(_mapper));
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] SystemAppEditForm form)
   {
      var entity = await _systemAppService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(form, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      form.SetValuesTo(entity);
      entity.SetUpdated(User.Id());

      await _systemAppService.UpdateAsync(entity, User.Id());

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _systemAppService.GetByIdAsync(id);
      if (entity == null) return NotFound();
     
      await _systemAppService.RemoveAsync(entity, User.Id());

      return NoContent();
   }
   async Task ValidateRequestAsync(SystemAppBaseForm form, int id)
   {
      var labels = new SystemAppLabels();
     
      if (!ModelState.IsValid) return;

      //var server = await _serverService.GetByIdAsync(form.ServerId);
      //if (server == null)
      //{
      //   ModelState.AddModelError(nameof(form.ServerId), ValidationMessages.NotExist($"{labels.Server} id = {id}"));
      //}
   }


}