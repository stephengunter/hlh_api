using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class DbBackupPlansHelpers
{

   public static DbBackupPlanViewModel MapViewModel(this DbBackupPlan entity, IMapper mapper)
   {
      
      var model = mapper.Map<DbBackupPlanViewModel>(entity);
      return model;
   }

   public static List<DbBackupPlanViewModel> MapViewModelList(this IEnumerable<DbBackupPlan> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<DbBackupPlan, DbBackupPlanViewModel> GetPagedList(this IEnumerable<DbBackupPlan> entities, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<DbBackupPlan, DbBackupPlanViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static DbBackupPlan MapEntity(this DbBackupPlanViewModel model, IMapper mapper, string currentUserId, DbBackupPlan? entity = null)
   {
      if (entity == null) entity = mapper.Map<DbBackupPlanViewModel, DbBackupPlan>(model);
      else entity = mapper.Map<DbBackupPlanViewModel, DbBackupPlan>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<DbBackupPlan> GetOrdered(this IEnumerable<DbBackupPlan> calendars)
     => calendars.OrderBy(item => item.Order);
}
