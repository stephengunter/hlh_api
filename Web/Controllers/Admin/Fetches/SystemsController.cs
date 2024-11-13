using ApplicationCore.Helpers;
using ApplicationCore.Helpers.Fetches;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Infrastructure.Helpers;
using ApplicationCore.Services.Fetches;
using ApplicationCore.Views.Fetches;

namespace Web.Controllers.Admin.Fetches;

[Route("admin/fetches/[controller]")]
public class SystemsController : BaseAdminController
{
   
   private readonly IMapper _mapper;
   private readonly IFetchesSystemService _systemService;

   public SystemsController(IFetchesSystemService systemService,IMapper mapper)
   {
      _systemService = systemService;
      _mapper = mapper;
   }

   [HttpGet]
   public async Task<ActionResult<IEnumerable<FetchesSystemView>>> Index()
   {
      var systems = await _systemService.FetchAllAsync();

      return systems.MapViewModelList(_mapper);
   }

   [HttpPost]
   public async Task<ActionResult<IEnumerable<FetchesSystemView>>> Store([FromBody] FetchesSystemView model)
   {
      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = model.MapEntity(_mapper);
      entity.Title = entity.Title.Trim();

      entity = await _systemService.CreateAsync(entity);
      var systems = await _systemService.FetchAllAsync();

      return systems.MapViewModelList(_mapper);

   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<FetchesSystemView>> Edit(int id)
   {
      var entity = await _systemService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      return entity.MapViewModel(_mapper);
   }


   [HttpPut("{id}")]
   public async Task<ActionResult<IEnumerable<FetchesSystemView>>> Update(int id, [FromBody] FetchesSystemView model)
   {
      var entity = await _systemService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);
      await _systemService.UpdateAsync(entity);

      var systems = await _systemService.FetchAllAsync();

      return systems.MapViewModelList(_mapper);
   }
   async Task ValidateRequestAsync(FetchesSystemView model, int id = 0)
   {
      if (string.IsNullOrEmpty(model.Department))
      {
         ModelState.AddModelError("department", "必須填寫部門.");
         return;
      }
      if (string.IsNullOrEmpty(model.Title))
      {
         ModelState.AddModelError("title", "必須填寫名稱.");
         return;
      }
      string title = model.Title.Trim();
      if (string.IsNullOrEmpty(title))
      {
         ModelState.AddModelError("title", "必須填寫名稱.");
         return;
      }
      var exist = await _systemService.FindByTitleAsync(title);
      if (exist != null && exist.Id != id)
      {
         ModelState.AddModelError("title", "名稱重複了.");
      }

   }
}