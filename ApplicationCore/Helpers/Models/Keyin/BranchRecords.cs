using AutoMapper;
using Infrastructure.Paging;
using ApplicationCore.Models.Keyin;
using ApplicationCore.Views.Keyin;

namespace ApplicationCore.Helpers.Keyin;
public static class BranchRecordHelpers
{
   public static BranchRecordView MapViewModel(this BranchRecord keyinRecord, IMapper mapper)
   {
      var model = mapper.Map<BranchRecordView>(keyinRecord);
      if (keyinRecord.Branch != null) model.Branch = keyinRecord.Branch.MapViewModel(mapper);
      return model;
   }


   public static List<BranchRecordView> MapViewModelList(this IEnumerable<BranchRecord> keyinRecords, IMapper mapper)
      => keyinRecords.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<BranchRecord, BranchRecordView> GetPagedList(this IEnumerable<BranchRecord> keyinRecords, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<BranchRecord, BranchRecordView>(keyinRecords, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static BranchRecord MapEntity(this BranchRecordView model, IMapper mapper, BranchRecord? entity = null)
   {
      if (entity == null) entity = mapper.Map<BranchRecordView, BranchRecord>(model);
      else entity = mapper.Map<BranchRecordView, BranchRecord>(model, entity);

      return entity;
   }
   public static BranchRecordReportItem MapReportItem(this BranchRecordView view)
      => new BranchRecordReportItem(view);

   public static List<BranchRecordReportItem> MapReportItemList(this IEnumerable<BranchRecordView> views)
     => views.Select(item => MapReportItem(item)).ToList();
}
