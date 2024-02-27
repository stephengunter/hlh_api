using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using System;

namespace ApplicationCore.Helpers;
public static class ArticleHelpers
{
   public static ArticleViewModel MapViewModel(this Article article, IMapper mapper)
   {
      var model = mapper.Map<ArticleViewModel>(article);
      
      return model;
   }


   public static List<ArticleViewModel> MapViewModelList(this IEnumerable<Article> articles, IMapper mapper)
      => articles.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Article, ArticleViewModel> GetPagedList(this IEnumerable<Article> articles, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Article, ArticleViewModel>(articles, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Article MapEntity(this ArticleViewModel model, IMapper mapper, string currentUserId, Article? entity = null)
   {
      if (entity == null) entity = mapper.Map<ArticleViewModel, Article>(model);
      else entity = mapper.Map<ArticleViewModel, Article>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Article> GetOrdered(this IEnumerable<Article> articles)
     => articles.OrderByDescending(item => item.CreatedAt);
}
