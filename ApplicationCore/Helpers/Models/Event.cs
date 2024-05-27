using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class EventHelpers
{

   public static EventViewModel MapViewModel(this Event entity, IMapper mapper)
   {
      var model = mapper.Map<EventViewModel>(entity);
      return model;
   }

   public static List<EventViewModel> MapViewModelList(this IEnumerable<Event> events, IMapper mapper)
      => events.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Event, EventViewModel> GetPagedList(this IEnumerable<Event> events, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Event, EventViewModel>(events, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Event MapEntity(this EventViewModel model, IMapper mapper, string currentUserId, Event? entity = null)
   {
      if (entity == null) entity = mapper.Map<EventViewModel, Event>(model);
      else entity = mapper.Map<EventViewModel, Event>(model, entity);

      //entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Event> GetOrdered(this IEnumerable<Event> events)
     => events.OrderByDescending(item => item.CreatedAt);
}
