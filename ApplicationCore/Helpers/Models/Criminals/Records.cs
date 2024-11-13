using AutoMapper;
using Infrastructure.Paging;
using ApplicationCore.Models.Criminals;
using ApplicationCore.Views.Criminals;
using ApplicationCore.Models;

namespace ApplicationCore.Helpers.Criminals;
public static class CriminalFetchRecordHelpers
{
   public static CriminalFetchRecordView MapViewModel(this CriminalFetchRecord record, IMapper mapper)
   {
      var model = mapper.Map<CriminalFetchRecordView>(record);
      return model;
   }


   public static List<CriminalFetchRecordView> MapViewModelList(this IEnumerable<CriminalFetchRecord> records, IMapper mapper)
      => records.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<CriminalFetchRecord, CriminalFetchRecordView> GetPagedList(this IEnumerable<CriminalFetchRecord> records, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<CriminalFetchRecord, CriminalFetchRecordView>(records, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static CriminalFetchRecord MapEntity(this CriminalFetchRecordView model, IMapper mapper, CriminalFetchRecord? entity = null)
   {
      if (entity == null) entity = mapper.Map<CriminalFetchRecordView, CriminalFetchRecord>(model);
      else entity = mapper.Map<CriminalFetchRecordView, CriminalFetchRecord>(model, entity);

      return entity;
   }
   public static List<CriminalFetchRecord> MapEntityList(this IEnumerable<CriminalFetchRecordView> views, IMapper mapper)
     => views.Select(item => MapEntity(item, mapper)).ToList();

   public static IEnumerable<CriminalFetchRecord> GetOrdered(this IEnumerable<CriminalFetchRecord> records)
    => records.OrderBy(item => item.Day).OrderBy(item => item.Id);

}
