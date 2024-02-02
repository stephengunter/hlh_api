using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using System;

namespace ApplicationCore.Helpers;
public static class JobHelpers
{

   public static JobViewModel MapViewModel(this Job job, IMapper mapper)
   {
      var model = mapper.Map<JobViewModel>(job);
      model.SetBaseRecordViewValues();
      
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

      entity.SetBaseRecordValues(model);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Job> GetOrdered(this IEnumerable<Job> jobs)
     => jobs.OrderByDescending(item => item.CreatedAt);
}
