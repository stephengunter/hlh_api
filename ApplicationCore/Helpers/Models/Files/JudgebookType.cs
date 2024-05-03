using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views.Files;
using AutoMapper;
using Infrastructure.Paging;
using System;
using ApplicationCore.Models.Files;
using Ardalis.Specification;

namespace ApplicationCore.Helpers.Files;
public static class JudgebookTypeHelpers
{
   public static JudgebookTypeViewModel MapViewModel(this JudgebookType entity, IMapper mapper)
      => mapper.Map<JudgebookTypeViewModel>(entity);

   public static List<JudgebookTypeViewModel> MapViewModelList(this IEnumerable<JudgebookType> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<JudgebookType, JudgebookTypeViewModel> GetPagedList(this IEnumerable<JudgebookType> entities, IMapper mapper,
      int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<JudgebookType, JudgebookTypeViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static JudgebookType MapEntity(this JudgebookTypeViewModel model, IMapper mapper, string currentUserId, JudgebookType? entity = null)
   {
      if (entity == null) entity = mapper.Map<JudgebookTypeViewModel, JudgebookType>(model);
      else entity = mapper.Map<JudgebookTypeViewModel, JudgebookType>(model, entity);

      return entity;
   }

   public static IEnumerable<JudgebookType> GetOrdered(this IEnumerable<JudgebookType> entities)
     => entities.OrderBy(item => item.Order);
}
