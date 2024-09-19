using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;

namespace ApplicationCore.Helpers;
public static class BranchHelpers
{

   public static BranchViewModel MapViewModel(this Branch branch, IMapper mapper)
   {
      var model = mapper.Map<BranchViewModel>(branch);
      return model;
   }

   public static List<BranchViewModel> MapViewModelList(this IEnumerable<Branch> branchs, IMapper mapper)
      => branchs.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Branch, BranchViewModel> GetPagedList(this IEnumerable<Branch> branchs, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Branch, BranchViewModel>(branchs, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Branch MapEntity(this BranchViewModel model, IMapper mapper, string currentUserId, Branch? entity = null)
   {
      if (entity == null) entity = mapper.Map<BranchViewModel, Branch>(model);
      else entity = mapper.Map<BranchViewModel, Branch>(model, entity);
      return entity;
   }

   public static IEnumerable<Branch> GetOrdered(this IEnumerable<Branch> branchs)
     => branchs.OrderByDescending(item => item.Order);
}
