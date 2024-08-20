using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class DatabasesHelpers
{

   public static DatabaseViewModel MapViewModel(this Database entity, IMapper mapper)
   {
      var model = mapper.Map<DatabaseViewModel>(entity);
      return model;
   }

   public static List<DatabaseViewModel> MapViewModelList(this IEnumerable<Database> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Database, DatabaseViewModel> GetPagedList(this IEnumerable<Database> entities, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Database, DatabaseViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Database MapEntity(this DatabaseViewModel model, IMapper mapper, string currentUserId, Database? entity = null)
   {
      if (entity == null) entity = mapper.Map<DatabaseViewModel, Database>(model);
      else entity = mapper.Map<DatabaseViewModel, Database>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Database> GetOrdered(this IEnumerable<Database> calendars)
     => calendars.OrderBy(item => item.Order);
}
