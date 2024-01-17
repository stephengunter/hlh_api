using ApplicationCore.Exceptions;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using ApplicationCore.DtoMapper;
using Web.Filters;
using Web.Models;
using ApplicationCore.Consts;
using ApplicationCore.Models;
using Azure.Core;
using ApplicationCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

public class TagsController : BaseAdminController
{
   private readonly ITagsService _tagsService;
   private readonly IMapper _mapper;

  
   public TagsController(ITagsService tagsService, IMapper mapper)
   {
      _tagsService = tagsService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<PagedList<Tag, TagViewModel>>> Index(bool active, int page = 1, int pageSize = 10)
   {
      var tags = await _tagsService.FetchAllAsync();

      if (tags.HasItems())
      {
         tags = tags.Where(x => x.Active == active);

         tags = tags.GetOrdered().ToList();
      }
      return tags.GetPagedList(_mapper, page, pageSize);
   }


   [HttpGet("create")]
   public ActionResult<TagViewModel> Create() => new TagViewModel();


   [HttpPost]
   public async Task<ActionResult<TagViewModel>> Store([FromBody] TagViewModel model)
   {
      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var existEntity = _tagsService.FindByTitleAsync(model.Title);
      if (existEntity is not null)
      {
         ModelState.AddModelError("title", "名稱重複了");
         return BadRequest(ModelState);
      }

      var tag = model.MapEntity(_mapper, User.Id());
      tag.Order = model.Active ? 0 : -1;

      tag = await _tagsService.CreateAsync(tag);

      return Ok(tag.MapViewModel(_mapper));
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult> Edit(int id)
   {
      var tag = await _tagsService.GetByIdAsync(id);
      if (tag == null) return NotFound();

      var model = tag.MapViewModel(_mapper);

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] TagViewModel model)
   {
      var tag = await _tagsService.GetByIdAsync(id);
      if (tag == null) return NotFound();

      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var existEntity = _tagsService.FindByTitleAsync(model.Title);
      if (existEntity is not null && existEntity.Id != id)
      {
         ModelState.AddModelError("title", "名稱重複了");
         return BadRequest(ModelState);
      }

      tag = model.MapEntity(_mapper, User.Id(), tag);

      await _tagsService.UpdateAsync(tag);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var tag = await _tagsService.GetByIdAsync(id);
      if (tag == null) return NotFound();

      tag.Removed = true;
      tag.Order = -1;
      await _tagsService.UpdateAsync(tag);

      return NoContent();
   }

   void ValidateRequest(TagViewModel model)
   {
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫名稱");
   }


}