using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Entities;

namespace ApplicationCore.Services;

public interface IBranchesService
{
   Task<IEnumerable<Branch>> FetchAsync();

   Task<Branch?> FindByTitleAsync(string title);
   Task<Branch?> GetByIdAsync(int id);

   Task<Branch> CreateAsync(Branch entity);
	Task UpdateAsync(Branch entity);
}

public class BranchesService : IBranchesService
{
	private readonly IDefaultRepository<Branch> _branchesRepository;

	public BranchesService(IDefaultRepository<Branch> branchesRepository)
	{
      _branchesRepository = branchesRepository;

   }

   public async Task<IEnumerable<Branch>> FetchAsync()
      => await _branchesRepository.ListAsync(new BranchSpecification());

   public async Task<Branch?> FindByTitleAsync(string title)
      => await _branchesRepository.FirstOrDefaultAsync(new BranchSpecification(title));

   public async Task<Branch?> GetByIdAsync(int id)
      => await _branchesRepository.GetByIdAsync(id);

   public async Task<Branch> CreateAsync(Branch entity)
		=> await _branchesRepository.AddAsync(entity);

		public async Task UpdateAsync(Branch entity)
		=> await _branchesRepository.UpdateAsync(entity);

}
