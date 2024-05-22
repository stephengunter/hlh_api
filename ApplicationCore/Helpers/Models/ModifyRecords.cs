using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class ModifyRecordHelpers
{
   public static ModifyRecordViewModel MapViewModel(this ModifyRecord modifyRecord, IMapper mapper)
      => mapper.Map<ModifyRecordViewModel>(modifyRecord);

   public static List<ModifyRecordViewModel> MapViewModelList(this IEnumerable<ModifyRecord> modifyRecords, IMapper mapper)
      => modifyRecords.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<ModifyRecord, ModifyRecordViewModel> GetPagedList(this IEnumerable<ModifyRecord> modifyRecords, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<ModifyRecord, ModifyRecordViewModel>(modifyRecords, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static IEnumerable<ModifyRecord> GetOrdered(this IEnumerable<ModifyRecord> modifyRecords)
     => modifyRecords.OrderByDescending(item => item.DateTime);
}
