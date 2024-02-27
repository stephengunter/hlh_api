using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class JobHelpers
{
   public static string RoleText(this JobRole role)
   {
      if (role == JobRole.Normal) return "一般";
      if (role == JobRole.Vice) return "副主管";
      if (role == JobRole.Chief) return "主管";
      return "";
   }

   public static JobViewModel MapViewModel(this Job job, IMapper mapper)
   {
      var model = mapper.Map<JobViewModel>(job);
      model.RoleText = job.Role.RoleText();


      return model;
   }


   public static List<JobViewModel> MapViewModelList(this IEnumerable<Job> jobs, IMapper mapper)
      => jobs.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Job, JobViewModel> GetPagedList(this IEnumerable<Job> jobs, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Job, JobViewModel>(jobs, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Job MapEntity(this JobViewModel model, IMapper mapper, string currentUserId, Job? entity = null)
   {
      if (entity == null) entity = mapper.Map<JobViewModel, Job>(model);
      else entity = mapper.Map<JobViewModel, Job>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Job> GetOrdered(this IEnumerable<Job> jobs)
     => jobs.OrderByDescending(item => item.CreatedAt);
}
