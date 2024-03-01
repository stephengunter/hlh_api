using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Microsoft.Extensions.Primitives;
using System;
using Infrastructure.Paging;
using ApplicationCore.Consts;

namespace ApplicationCore.Helpers;
public static class LocationHelpers
{
   public static LocationViewModel MapViewModel(this Location location, IMapper mapper)
   {
      var model = mapper.Map<LocationViewModel>(location);
      
      return model;
   }

   public static List<LocationViewModel> MapViewModelList(this IEnumerable<Location> locations, IMapper mapper)
      => locations.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Location, LocationViewModel> GetPagedList(this IEnumerable<Location> locations, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Location, LocationViewModel>(locations, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Location MapEntity(this LocationViewModel model, IMapper mapper, string currentUserId, Location? entity = null)
   {
      if (entity == null) entity = mapper.Map<LocationViewModel, Location>(model);
      else entity = mapper.Map<LocationViewModel, Location>(model, entity);

      entity.SetActive(model.Active);

      return entity;
   }

   public static IEnumerable<Location> GetOrdered(this IEnumerable<Location> locations)
     => locations.OrderByDescending(item => item.Order);

   public static Location? FindByName(this IEnumerable<Location> locations, string name)
     => locations.FirstOrDefault(item => item.Title == name);

   public static Location? FindByKey(this IEnumerable<Location> locations, string key)
     => locations.FirstOrDefault(item => item.Key == key);
}
