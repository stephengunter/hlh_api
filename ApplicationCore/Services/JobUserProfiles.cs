using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ApplicationCore.Consts;
using ApplicationCore.Specifications;
using Infrastructure.Interfaces;

namespace ApplicationCore.Services;

public interface IJobUserProfilessService
{
   Task<IEnumerable<JobUserProfiles>> FetchAsync(Job job);
   Task<IEnumerable<JobUserProfiles>> FetchAsync(User user);
   Task<JobUserProfiles?> GetByIdAsync(int id);
   Task<JobUserProfiles?> DetailsAsync(int id);

   Task<JobUserProfiles> CreateAsync(JobUserProfiles entity);
   Task UpdateAsync(JobUserProfiles entity);
}

public class JobUserProfilessService : IJobUserProfilessService
{
   private readonly IDefaultRepository<JobUserProfiles> _repository;

   public JobUserProfilessService(IDefaultRepository<JobUserProfiles> jobsRepository)
   {
      _repository = jobsRepository;
   }
   public async Task<IEnumerable<JobUserProfiles>> FetchAsync(Job job)
      => await _repository.ListAsync(new JobUserProfilesSpecification(job));

   public async Task<IEnumerable<JobUserProfiles>> FetchAsync(User user)
      => await _repository.ListAsync(new JobUserProfilesSpecification(user));

   public async Task<JobUserProfiles?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<JobUserProfiles?> DetailsAsync(int id)
      => await _repository.FirstOrDefaultAsync(new JobUserProfilesSpecification(id));

   public async Task<JobUserProfiles> CreateAsync(JobUserProfiles JobUserProfiles)
      => await _repository.AddAsync(JobUserProfiles);

   public async Task UpdateAsync(JobUserProfiles JobUserProfiles)
   => await _repository.UpdateAsync(JobUserProfiles);

}
