using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;
using ApplicationCore.Services;

namespace ApplicationCore.Helpers;
public static class ModifyRecordHelpers
{
   public static ModifyRecordViewModel MapViewModel(this ModifyRecord modifyRecord, IMapper mapper)
      => mapper.Map<ModifyRecordViewModel>(modifyRecord);

   public static List<ModifyRecordViewModel> MapViewModelList(this IEnumerable<ModifyRecord> modifyRecords, IMapper mapper)
      => modifyRecords.Select(item => MapViewModel(item, mapper)).ToList();

   public static async Task<List<ModifyRecordViewModel>> MapViewModelListAsync(this IEnumerable<ModifyRecord>? modifyRecords, IUsersService usersService, IMapper mapper)
   {
      var modelList = new List<ModifyRecordViewModel>();
      if (modifyRecords.IsNullOrEmpty()) return modelList;

      var userIds = modifyRecords!.Select(x => x.UserId).Distinct();
      var users = await usersService.FetchByIdsAsync(userIds);
      modelList = modifyRecords.GetOrdered().MapViewModelList(mapper);
      foreach (var user in users)
      {
         var models = modelList.Where(x => x.UserId == user.Id);
         foreach (var model in models)
         {
            model!.UserName = user.UserName!;
            if (user.Profiles != null) model.Profiles = user.Profiles.MapViewModel(mapper);

         }
      }
      return modelList;
   }
   public static PagedList<ModifyRecord, ModifyRecordViewModel> GetPagedList(this IEnumerable<ModifyRecord> modifyRecords, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<ModifyRecord, ModifyRecordViewModel>(modifyRecords, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static IEnumerable<ModifyRecord> GetOrdered(this IEnumerable<ModifyRecord> modifyRecords)
     => modifyRecords.OrderByDescending(item => item.DateTime);
}
