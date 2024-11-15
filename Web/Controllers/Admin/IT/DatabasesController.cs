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

namespace Web.Controllers.Admin.IT;

public class DatabasesController : BaseAdminITController
{
   private readonly IDatabaseService _databaseService;
   private readonly IMapper _mapper;
     
   public DatabasesController(IDatabaseService databaseService, IMapper mapper)
   {
      _databaseService = databaseService;
      _mapper = mapper;
   }
   [HttpGet("init")]
   public async Task<ActionResult<DatabasesIndexModel>> Init()
   {
      int page = 1;
      int pageSize = 10;
      var request = new DatabasesFetchRequest(page, pageSize);
      var providers = new List<string>() { DbProvider.SQLServer, DbProvider.PostgreSql };

      return new DatabasesIndexModel(request, providers);
   }
   [HttpGet]
   public async Task<ActionResult<PagedList<Database, DatabaseViewModel>>> Index(bool active, int page = 1, int pageSize = 10)
   {
      var list = await _databaseService.FetchAsync();

      if (list.HasItems())
      {
         list = list.Where(x => x.Active == active);

         list = list.GetOrdered().ToList();
      }
      return list.GetPagedList(_mapper, page, pageSize);
   }


   [HttpGet("create")]
   public ActionResult<DatabaseAddForm> Create() => new DatabaseAddForm() { Provider = DbProvider.SQLServer };


   [HttpPost]
   public async Task<ActionResult<DatabaseViewModel>> Store([FromBody] DatabaseViewModel model)
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

      entity = await _databaseService.CreateAsync(entity, User.Id());

      return Ok(entity.MapViewModel(_mapper));
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult> Edit(int id)
   {
      var entity = await _databaseService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var model = entity.MapViewModel(_mapper);

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] DatabaseViewModel model)
   {
      var entity = await _databaseService.GetByIdAsync(id);
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

   void ValidateRequest(DatabaseViewModel model)
   {
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫名稱");
   }


}