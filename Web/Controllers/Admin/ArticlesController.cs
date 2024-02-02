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
using Infrastructure.Helpers;

namespace Web.Controllers;

public class ArticlesController : BaseAdminController
{
   private readonly IArticlesService _articlesService;
   private readonly IMapper _mapper;

  
   public ArticlesController(IArticlesService articlesService, IMapper mapper)
   {
      _articlesService = articlesService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<PagedList<Article, ArticleViewModel>>> Index(int category, bool active, int page = 1, int pageSize = 10)
   {
      IEnumerable<Article> articles;
      if (category > 0) articles = await _articlesService.FetchAsync(new Category { Id = category });
      else articles = await _articlesService.FetchAllAsync();

      if (articles.HasItems())
      {
         articles = articles.Where(x => x.Active == active);

         articles = articles.GetOrdered().ToList();
      }
      return articles.GetPagedList(_mapper, page, pageSize);
   }


   [HttpGet("create")]
   public ActionResult<ArticleViewModel> Create() => new ArticleViewModel();


   [HttpPost]
   public async Task<ActionResult<ArticleViewModel>> Store([FromBody] ArticleViewModel model)
   {
      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var article = model.MapEntity(_mapper, User.Id());
      article.Order = model.Active ? 0 : -1;

      article = await _articlesService.CreateAsync(article);

      return Ok(article.MapViewModel(_mapper));
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult> Edit(int id)
   {
      var article = await _articlesService.GetByIdAsync(id);
      if (article == null) return NotFound();

      var model = article.MapViewModel(_mapper);

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] ArticleViewModel model)
   {
      var article = await _articlesService.GetByIdAsync(id);
      if (article == null) return NotFound();

      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      article = model.MapEntity(_mapper, User.Id(), article);

      await _articlesService.UpdateAsync(article);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var article = await _articlesService.GetByIdAsync(id);
      if (article == null) return NotFound();

      article.Removed = true;
      article.Order = -1;
      await _articlesService.UpdateAsync(article);

      return NoContent();
   }

   void ValidateRequest(ArticleViewModel model)
   {
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫標題");

      if (String.IsNullOrEmpty(model.Content)) ModelState.AddModelError("content", "必須填寫內容");

      if (model.Active) 
      {
         if (String.IsNullOrEmpty(model.Cover)) ModelState.AddModelError("content", "必須有封面圖");
      }

      

   }


}