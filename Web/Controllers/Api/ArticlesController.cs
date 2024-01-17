
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;

namespace Web.Controllers.Api
{
   
   public class ArticlesController : BaseApiController
   {
      private readonly IArticlesService _articlesService;
      private readonly IMapper _mapper;

      public ArticlesController(IArticlesService articlesService, IMapper mapper)
      {
         _articlesService = articlesService;
         _mapper = mapper;
      }


      [HttpGet("")]
      public async Task<ActionResult<IEnumerable<ArticleViewModel>>> Index(int page = 1, int pageSize = 99)
      {
         if (page < 1) page = 1;

         var articles = await _articlesService.FetchAllAsync();

         articles = articles.Where(x => x.Active);

         articles = articles.GetOrdered().GetPaged(page, pageSize);

         return articles.MapViewModelList(_mapper);
      }


      [HttpGet("{id}")]
      public async Task<ActionResult<ArticleViewModel>> Details(int id)
      {
         var article = await _articlesService.GetByIdAsync(id);
         if (article == null) return NotFound();
         if (!article.Active) return NotFound();

         return article.MapViewModel(_mapper);
      }

   }


}