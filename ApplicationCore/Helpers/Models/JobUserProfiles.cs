using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.Helpers;
public static class JobUserProfilesHelpers
{
   public static JobUserProfilesViewModel MapViewModel(this JobUserProfiles entity, IMapper mapper)
   {
      var model = mapper.Map<JobUserProfilesViewModel>(entity);
      model.Status = Convert.ToInt32(entity.Status);
      model.StatusText = entity.Status.ToText();
      model.StartDateText = entity.StartDate.ToDateTimeString();
      model.EndDateText = entity.EndDate.ToDateTimeString();
      return model;
   }


   public static List<JobUserProfilesViewModel> MapViewModelList(this IEnumerable<JobUserProfiles> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static JobUserProfiles MapEntity(this JobUserProfilesViewModel model, IMapper mapper, string currentUserId, JobUserProfiles? entity = null)
   {
      if (entity == null) entity = mapper.Map<JobUserProfilesViewModel, JobUserProfiles>(model);
      else entity = mapper.Map<JobUserProfilesViewModel, JobUserProfiles>(model, entity);

      entity.StartDate = model.StartDateText.ToStartDate();
      entity.EndDate = model.StartDateText.ToEndDate();

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<JobUserProfiles> GetOrdered(this IEnumerable<JobUserProfiles> jobs)
     => jobs.OrderByDescending(item => item.CreatedAt);
}
