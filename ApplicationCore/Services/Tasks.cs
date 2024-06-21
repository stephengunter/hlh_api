using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApplicationCore.Services;

public interface ITaskService
{
   Task<IEnumerable<Tasks>> FindRootAsync();
   Task<IEnumerable<Tasks>> FetchAllAsync();
   Task<Tasks?> GetByIdAsync(int id);

   Task<Tasks> CreateAsync(Tasks task);
	Task UpdateAsync(Tasks task);
}

public class TaskService : ITaskService
{
	private readonly IDefaultRepository<Tasks> _taskRepository;

	public TaskService(IDefaultRepository<Tasks> taskRepository)
	{
      _taskRepository = taskRepository;
	}
   public async Task<IEnumerable<Tasks>> FindRootAsync()
       => await _taskRepository.ListAsync(new RootTaskSpecification());

   public async Task<IEnumerable<Tasks>> FetchAllAsync()
      => await _taskRepository.ListAsync(new TaskSpecification());

   public async Task<Tasks?> GetByIdAsync(int id)
      => await _taskRepository.GetByIdAsync(id);

   public async Task<Tasks> CreateAsync(Tasks task)
		=> await _taskRepository.AddAsync(task);

		public async Task UpdateAsync(Tasks task)
		=> await _taskRepository.UpdateAsync(task);

}
