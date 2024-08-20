using ApplicationCore.Services;
using ApplicationCore.Views.IT;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models.IT;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Infrastructure.Paging;

namespace Web.Controllers.Admin.IT;

public class SystemAppsController : BaseAdminITController
{
   private readonly ISystemAppService _systemAppService;
   private readonly IMapper _mapper;
     
   public SystemAppsController(ISystemAppService systemAppService, IMapper mapper)
   {
      _systemAppService = systemAppService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<PagedList<SystemApp, SystemAppViewModel>>> Index(bool active, int page = 1, int pageSize = 10)
   {
      var list = await _systemAppService.FetchAsync();

      if (list.HasItems())
      {
         list = list.Where(x => x.Active == active);

         list = list.GetOrdered().ToList();
      }
      return list.GetPagedList(_mapper, page, pageSize);
   }


   [HttpGet("create")]
   public ActionResult<SystemAppViewModel> Create() => new SystemAppViewModel();


   [HttpPost]
   public async Task<ActionResult<SystemAppViewModel>> Store([FromBody] SystemAppViewModel model)
   {
      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      //var existEntity = _tagsService.FindByTitleAsync(model.Title);
      //if (existEntity is not null)
      //{
      //   ModelState.AddModelError("title", "名稱重複了");
      //   return BadRequest(ModelState);
      //}

      var entity = model.MapEntity(_mapper, User.Id());
      entity.Order = model.Active ? 0 : -1;

      entity = await _systemAppService.CreateAsync(entity, User.Id());

      return Ok(entity.MapViewModel(_mapper));
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult> Edit(int id)
   {
      var entity = await _systemAppService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var model = entity.MapViewModel(_mapper);

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] SystemAppViewModel model)
   {
      var entity = await _systemAppService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      //var existEntity = _tagsService.FindByTitleAsync(model.Title);
      //if (existEntity is not null && existEntity.Id != id)
      //{
      //   ModelState.AddModelError("title", "名稱重複了");
      //   return BadRequest(ModelState);
      //}

      entity = model.MapEntity(_mapper, User.Id(), entity);

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

   void ValidateRequest(SystemAppViewModel model)
   {
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫名稱");
   }


}