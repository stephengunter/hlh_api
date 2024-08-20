using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class HostsHelpers
{

   public static HostViewModel MapViewModel(this Host entity, IMapper mapper)
   {
      var model = mapper.Map<HostViewModel>(entity);
      return model;
   }

   public static List<HostViewModel> MapViewModelList(this IEnumerable<Host> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Host, HostViewModel> GetPagedList(this IEnumerable<Host> entities, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Host, HostViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Host MapEntity(this HostViewModel model, IMapper mapper, string currentUserId, Host? entity = null)
   {
      if (entity == null) entity = mapper.Map<HostViewModel, Host>(model);
      else entity = mapper.Map<HostViewModel, Host>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Host> GetOrdered(this IEnumerable<Host> calendars)
     => calendars.OrderBy(item => item.Order);
}
