using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using System;

namespace ApplicationCore.Helpers;
public static class CategoryHelpers
{
   public static CategoryViewModel MapViewModel(this Category category, IMapper mapper)
   {
      var model = mapper.Map<CategoryViewModel>(category);
      
      return model;
   }


   public static List<CategoryViewModel> MapViewModelList(this IEnumerable<Category> categorys, IMapper mapper)
      => categorys.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Category, CategoryViewModel> GetPagedList(this IEnumerable<Category> categorys, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Category, CategoryViewModel>(categorys, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Category MapEntity(this CategoryViewModel model, IMapper mapper, string currentUserId, Category? entity = null)
   {
      if (entity == null) entity = mapper.Map<CategoryViewModel, Category>(model);
      else entity = mapper.Map<CategoryViewModel, Category>(model, entity);

      entity.SetActive(model.Active);

      return entity;
   }

   public static IEnumerable<Category> GetOrdered(this IEnumerable<Category> categorys)
     => categorys.OrderBy(item => item.Order);
}
