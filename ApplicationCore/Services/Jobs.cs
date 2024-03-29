using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface IJobsService
{
   Task<IEnumerable<Job>> FetchAsync();
   Task<IEnumerable<Job>> FetchAsync(IEnumerable<Department> departments);
   Task<Job?> GetByIdAsync(int id);
   Task<Job?> DetailsAsync(int id);

   Task<Job> CreateAsync(Job Job);
	Task UpdateAsync(Job Job);

   Task<IEnumerable<JobTitle>> FetchJobTitlesAsync();
}

public class JobsService : IJobsService
{
	private readonly IDefaultRepository<Job> _jobsRepository;
   private readonly IDefaultRepository<JobTitle> _jobTitlesRepository;

   public JobsService(IDefaultRepository<Job> jobsRepository, IDefaultRepository<JobTitle> jobTitlesRepository)
	{
      _jobsRepository = jobsRepository;
      _jobTitlesRepository = jobTitlesRepository;
   }
   public async Task<IEnumerable<Job>> FetchAsync()
      => await _jobsRepository.ListAsync(new JobSpecification());

   public async Task<IEnumerable<Job>> FetchAsync(IEnumerable<Department> departments)
      => await _jobsRepository.ListAsync(new JobSpecification(departments));

   public async Task<Job?> GetByIdAsync(int id)
      => await _jobsRepository.GetByIdAsync(id);

   public async Task<Job?> DetailsAsync(int id)
      => await _jobsRepository.FirstOrDefaultAsync(new JobSpecification(id));

   public async Task<Job> CreateAsync(Job Job)
		=> await _jobsRepository.AddAsync(Job);

		public async Task UpdateAsync(Job Job)
		=> await _jobsRepository.UpdateAsync(Job);

   public async Task<IEnumerable<JobTitle>> FetchJobTitlesAsync()
      => await _jobTitlesRepository.ListAsync();

}
