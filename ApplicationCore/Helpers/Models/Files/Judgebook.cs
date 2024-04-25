using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views.Files;
using AutoMapper;
using Infrastructure.Paging;
using System;
using ApplicationCore.Models.Files;
using Ardalis.Specification;

namespace ApplicationCore.Helpers.Files;
public static class JudgebookFileHelpers
{
   public static JudgebookFileViewModel MapViewModel(this JudgebookFile entity, IMapper mapper)
   {
      var model = mapper.Map<JudgebookFileViewModel>(entity);
      return model;
   }


   public static List<JudgebookFileViewModel> MapViewModelList(this IEnumerable<JudgebookFile> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<JudgebookFile, JudgebookFileViewModel> GetPagedList(this IEnumerable<JudgebookFile> entities, IMapper mapper, 
      int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<JudgebookFile, JudgebookFileViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static JudgebookFile MapEntity(this JudgebookFileViewModel model, IMapper mapper, string currentUserId, JudgebookFile? entity = null)
   {
      if (entity == null) entity = mapper.Map<JudgebookFileViewModel, JudgebookFile>(model);
      else entity = mapper.Map<JudgebookFileViewModel, JudgebookFile>(model, entity);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<JudgebookFile> GetOrdered(this IEnumerable<JudgebookFile> entities)
     => entities.OrderByDescending(item => item.CreatedAt);
}
