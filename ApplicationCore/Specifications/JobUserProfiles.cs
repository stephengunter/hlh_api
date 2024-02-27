using Ardalis.Specification;
using ApplicationCore.Models;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class JobUserProfilesSpecification : Specification<JobUserProfiles>
{
   public JobUserProfilesSpecification(Job job, string include = "user")
   {
      if (include.EqualTo("job")) Query.Include(item => item.Job).Where(item => !item.Removed && item.JobId == job.Id);
      else if (include.EqualTo("user")) Query.Include(item => item.UserProfiles).Where(item => !item.Removed && item.JobId == job.Id);
      else if (String.IsNullOrEmpty(include)) Query.Where(item => !item.Removed && item.JobId == job.Id);
   }
   public JobUserProfilesSpecification(User user, string include = "job")
   {
      if (include.EqualTo("job")) Query.Include(item => item.Job).Where(item => !item.Removed && item.UserId == user.Id);
      else if (include.EqualTo("user")) Query.Include(item => item.UserProfiles).Where(item => !item.Removed && item.UserId == user.Id);
      else if (String.IsNullOrEmpty(include)) Query.Where(item => !item.Removed && item.UserId == user.Id);
   }
   public JobUserProfilesSpecification(int id, string include = "user")
   {
      if (include.EqualTo("job")) Query.Include(item => item.Job).Where(item => !item.Removed && item.Id == id);
      else if (include.EqualTo("user")) Query.Include(item => item.UserProfiles).Where(item => !item.Removed && item.Id == id);
      else if (String.IsNullOrEmpty(include)) Query.Where(item => !item.Removed && item.Id == id);
   }
}

