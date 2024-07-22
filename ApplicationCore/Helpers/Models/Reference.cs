using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using System;

namespace ApplicationCore.Helpers;
public static class ReferenceHelpers
{
   public static ReferenceViewModel MapViewModel(this Reference entity, IMapper mapper)
   {
      var model = mapper.Map<ReferenceViewModel>(entity);
      if (entity.Attachment != null) model.Attachment = entity.Attachment.MapViewModel(mapper);
      return model;
   }


   public static List<ReferenceViewModel> MapViewModelList(this IEnumerable<Reference> references, IMapper mapper)
      => references.Select(item => MapViewModel(item, mapper)).ToList();

   public static Reference MapEntity(this ReferenceViewModel model, IMapper mapper, string currentUserId, Reference? entity = null)
   {
      if (entity == null) entity = mapper.Map<ReferenceViewModel, Reference>(model);
      else entity = mapper.Map<ReferenceViewModel, Reference>(model, entity);

      return entity;
   }

   public static IEnumerable<Reference> GetOrdered(this IEnumerable<Reference> articles)
     => articles.OrderByDescending(item => item.Order);
}
