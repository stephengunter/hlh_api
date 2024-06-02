using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class CalendarsHelpers
{

   public static CalendarViewModel MapViewModel(this Calendar entity, IMapper mapper)
   {
      var model = mapper.Map<CalendarViewModel>(entity);
      return model;
   }

   public static List<CalendarViewModel> MapViewModelList(this IEnumerable<Calendar> calendars, IMapper mapper)
      => calendars.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Calendar, CalendarViewModel> GetPagedList(this IEnumerable<Calendar> calendars, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Calendar, CalendarViewModel>(calendars, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Calendar MapEntity(this CalendarViewModel model, IMapper mapper, string currentUserId, Calendar? entity = null)
   {
      if (entity == null) entity = mapper.Map<CalendarViewModel, Calendar>(model);
      else entity = mapper.Map<CalendarViewModel, Calendar>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Calendar> GetOrdered(this IEnumerable<Calendar> calendars)
     => calendars.OrderBy(item => item.Order);
}
