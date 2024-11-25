using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface IDatabaseService
{
   Task<IEnumerable<Database>> FetchAsync(string include = "");
   Task<Database?> GetByIdAsync(int id, string include = "");
   Task<Database> CreateAsync(Database entity, string userId);
   Task UpdateAsync(Database entity, string userId);
   Task RemoveAsync(Database entity, string userId);
}

public class DatabaseService : IDatabaseService
{
	private readonly IDefaultRepository<Database> _databaseRepository;

	public DatabaseService(IDefaultRepository<Database> databaseRepository)
	{
      _databaseRepository = databaseRepository;
	}
   public async Task<IEnumerable<Database>> FetchAsync(string include = "")
       => await _databaseRepository.ListAsync(new DatabaseSpecification(include));


   public async Task<Database?> GetByIdAsync(int id, string include = "")
      => await _databaseRepository.FirstOrDefaultAsync(new DatabaseSpecification(id, include));


   public async Task<Database> CreateAsync(Database entity, string userId)
   {
      entity.SetCreated(userId);
      return await _databaseRepository.AddAsync(entity);
   }

   public async Task UpdateAsync(Database entity, string userId)
   {
      entity.SetUpdated(userId);
      await _databaseRepository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(Database entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _databaseRepository.UpdateAsync(entity);
   }

}
