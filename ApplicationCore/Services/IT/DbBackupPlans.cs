using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface IDbBackupPlanService
{
   Task<IEnumerable<DbBackupPlan>> FetchAsync(Database db, ICollection<string>? includes = null);
   Task<DbBackupPlan?> GetByIdAsync(int id);
   Task<DbBackupPlan> CreateAsync(DbBackupPlan entity, string userId);
   Task UpdateAsync(DbBackupPlan entity, string userId);
   Task RemoveAsync(DbBackupPlan entity, string userId);
}

public class DbBackupPlanService : IDbBackupPlanService
{
	private readonly IDefaultRepository<DbBackupPlan> _databaseRepository;

	public DbBackupPlanService(IDefaultRepository<DbBackupPlan> databaseRepository)
	{
      _databaseRepository = databaseRepository;
	}
   public async Task<IEnumerable<DbBackupPlan>> FetchAsync(Database db, ICollection<string>? includes = null)
       => await _databaseRepository.ListAsync(new DbBackupPlanSpecification(db, includes));


   public async Task<DbBackupPlan?> GetByIdAsync(int id)
      => await _databaseRepository.GetByIdAsync(id);


   public async Task<DbBackupPlan> CreateAsync(DbBackupPlan entity, string userId)
   {
      entity.SetCreated(userId);
      return await _databaseRepository.AddAsync(entity);
   }

   public async Task UpdateAsync(DbBackupPlan entity, string userId)
   {
      entity.SetUpdated(userId);
      await _databaseRepository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(DbBackupPlan entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _databaseRepository.UpdateAsync(entity);
   }

}
