using AutoMapper;
using Infrastructure.Paging;
using ApplicationCore.Models.Keyin;
using ApplicationCore.Views.Keyin;

namespace ApplicationCore.Helpers.Keyin;
public static class PersonRecordHelpers
{
   public static PersonRecordView MapViewModel(this PersonRecord keyinRecord, IMapper mapper)
   {
      var model = mapper.Map<PersonRecordView>(keyinRecord);
      if (keyinRecord.Person != null) model.Person = keyinRecord.Person.MapViewModel(mapper);
      return model;
   }


   public static List<PersonRecordView> MapViewModelList(this IEnumerable<PersonRecord> keyinRecords, IMapper mapper)
      => keyinRecords.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<PersonRecord, PersonRecordView> GetPagedList(this IEnumerable<PersonRecord> keyinRecords, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<PersonRecord, PersonRecordView>(keyinRecords, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static PersonRecord MapEntity(this PersonRecordView model, IMapper mapper, PersonRecord? entity = null)
   {
      if (entity == null) entity = mapper.Map<PersonRecordView, PersonRecord>(model);
      else entity = mapper.Map<PersonRecordView, PersonRecord>(model, entity);

      return entity;
   }

   public static PersonRecordReportItem MapReportItem(this PersonRecordView view)
      => new PersonRecordReportItem(view);

   public static List<PersonRecordReportItem> MapReportItemList(this IEnumerable<PersonRecordView> views)
     => views.Select(item => MapReportItem(item)).ToList();
}
