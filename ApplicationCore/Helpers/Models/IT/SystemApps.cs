using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class SystemAppsHelpers
{

   public static SystemAppViewModel MapViewModel(this SystemApp entity, IMapper mapper)
   {
      var model = mapper.Map<SystemAppViewModel>(entity);
      return model;
   }

   public static List<SystemAppViewModel> MapViewModelList(this IEnumerable<SystemApp> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<SystemApp, SystemAppViewModel> GetPagedList(this IEnumerable<SystemApp> entities, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<SystemApp, SystemAppViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static SystemApp MapEntity(this SystemAppViewModel model, IMapper mapper, string currentUserId, SystemApp? entity = null)
   {
      if (entity == null) entity = mapper.Map<SystemAppViewModel, SystemApp>(model);
      else entity = mapper.Map<SystemAppViewModel, SystemApp>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<SystemApp> GetOrdered(this IEnumerable<SystemApp> calendars)
     => calendars.OrderBy(item => item.Order);
}
