using AutoMapper;
using Infrastructure.Paging;
using ApplicationCore.Models.Keyin;
using ApplicationCore.Views.Keyin;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers.Keyin;
public static class KeyinPersonHelpers
{
   public static KeyinPersonView MapViewModel(this KeyinPerson keyinPerson, IMapper mapper)
   {
      var model = mapper.Map<KeyinPersonView>(keyinPerson);
      model.LeaveAtText = keyinPerson.LeaveAt.ToDateString();
      return model;
   }


   public static List<KeyinPersonView> MapViewModelList(this IEnumerable<KeyinPerson> keyinPersons, IMapper mapper)
      => keyinPersons.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<KeyinPerson, KeyinPersonView> GetPagedList(this IEnumerable<KeyinPerson> keyinPersons, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<KeyinPerson, KeyinPersonView>(keyinPersons, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static KeyinPerson MapEntity(this KeyinPersonView model, IMapper mapper, KeyinPerson? entity = null)
   {
      if (entity == null) entity = mapper.Map<KeyinPersonView, KeyinPerson>(model);
      else entity = mapper.Map<KeyinPersonView, KeyinPerson>(model, entity);

      return entity;
   }

   public static PersonPassesReportItem MapReportItem(this KeyinPersonView view)
      => new PersonPassesReportItem(view);

   public static List<PersonPassesReportItem> MapReportItemList(this IEnumerable<KeyinPersonView> views)
     => views.Select(item => MapReportItem(item)).ToList();
}
