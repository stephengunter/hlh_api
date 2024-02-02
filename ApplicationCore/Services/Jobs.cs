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

namespace ApplicationCore.Services;

public interface IJobsService
{
   Task<IEnumerable<Job>> FetchAsync();
   Task<IEnumerable<Job>> FetchAsync(IEnumerable<Department> departments);
   Task<Job?> GetByIdAsync(int id);

   Task<Job> CreateAsync(Job Job);
	Task UpdateAsync(Job Job);
}

public class JobsService : IJobsService
{
	private readonly IDefaultRepository<Job> _jobsRepository;

	public JobsService(IDefaultRepository<Job> jobsRepository)
	{
      _jobsRepository = jobsRepository;
	}
   public async Task<IEnumerable<Job>> FetchAsync()
      => await _jobsRepository.ListAsync(new JobSpecification());

   public async Task<IEnumerable<Job>> FetchAsync(IEnumerable<Department> departments)
      => await _jobsRepository.ListAsync(new JobSpecification(departments));

   public async Task<Job?> GetByIdAsync(int id)
      => await _jobsRepository.GetByIdAsync(id);

   public async Task<Job> CreateAsync(Job Job)
		=> await _jobsRepository.AddAsync(Job);

		public async Task UpdateAsync(Job Job)
		=> await _jobsRepository.UpdateAsync(Job);

}
