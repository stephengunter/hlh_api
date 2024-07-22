using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class TaskHelpers
{

   public static TaskViewModel MapViewModel(this Tasks entity, IMapper mapper)
   {
      var model = mapper.Map<TaskViewModel>(entity);
      if(entity.References.HasItems()) model.References = entity.References.MapViewModelList(mapper);

      model.SubItems = entity.SubItems.Select(x => x.MapViewModel(mapper)).ToList();

      return model;
   }

   public static List<TaskViewModel> MapViewModelList(this IEnumerable<Tasks> tasks, IMapper mapper)
      => tasks.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Tasks, TaskViewModel> GetPagedList(this IEnumerable<Tasks> tasks, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Tasks, TaskViewModel>(tasks, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Tasks MapEntity(this TaskViewModel model, IMapper mapper, string currentUserId, Tasks? entity = null)
   {
      if (entity == null) entity = mapper.Map<TaskViewModel, Tasks>(model);
      else entity = mapper.Map<TaskViewModel, Tasks>(model, entity);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Tasks> GetOrdered(this IEnumerable<Tasks> tasks)
     => tasks.OrderBy(item => item.Order);
}
