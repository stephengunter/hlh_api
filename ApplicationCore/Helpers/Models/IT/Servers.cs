using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class ServersHelpers
{

   public static ServerViewModel MapViewModel(this Server entity, IMapper mapper)
   {
      var model = mapper.Map<ServerViewModel>(entity);
      return model;
   }

   public static List<ServerViewModel> MapViewModelList(this IEnumerable<Server> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Server, ServerViewModel> GetPagedList(this IEnumerable<Server> entities, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Server, ServerViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Server MapEntity(this ServerViewModel model, IMapper mapper, string currentUserId, Server? entity = null)
   {
      if (entity == null) entity = mapper.Map<ServerViewModel, Server>(model);
      else entity = mapper.Map<ServerViewModel, Server>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Server> GetOrdered(this IEnumerable<Server> calendars)
     => calendars.OrderBy(item => item.Order);
}
