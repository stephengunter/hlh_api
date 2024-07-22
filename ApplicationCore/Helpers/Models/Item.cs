using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using System;

namespace ApplicationCore.Helpers;
public static class ItemHelpers
{
   public static ItemViewModel MapViewModel(this Item entity, IMapper mapper)
   {
      var model = mapper.Map<ItemViewModel>(entity);
      if (entity.Attachments.HasItems()) model.Attachments = entity.Attachments.MapViewModelList(mapper);
      return model;
   }


   public static List<ItemViewModel> MapViewModelList(this IEnumerable<Item> items, IMapper mapper)
      => items.Select(item => MapViewModel(item, mapper)).ToList();

   public static Item MapEntity(this ItemViewModel model, IMapper mapper, string currentUserId, Item? entity = null)
   {
      if (entity == null) entity = mapper.Map<ItemViewModel, Item>(model);
      else entity = mapper.Map<ItemViewModel, Item>(model, entity);

      return entity;
   }

   public static IEnumerable<Item> GetOrdered(this IEnumerable<Item> articles)
     => articles.OrderByDescending(item => item.Order);
}
