using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface ISystemAppService
{
   Task<IEnumerable<SystemApp>> FetchAsync();
   Task<SystemApp?> GetByIdAsync(int id);
   Task<SystemApp> CreateAsync(SystemApp entity, string userId);
   Task UpdateAsync(SystemApp entity, string userId);
   Task RemoveAsync(SystemApp entity, string userId);
}

public class SystemAppService : ISystemAppService
{
	private readonly IDefaultRepository<SystemApp> _systemappsRepository;

	public SystemAppService(IDefaultRepository<SystemApp> systemappsRepository)
	{
      _systemappsRepository = systemappsRepository;
	}
   public async Task<IEnumerable<SystemApp>> FetchAsync()
       => await _systemappsRepository.ListAsync(new SystemAppSpecification());


   public async Task<SystemApp?> GetByIdAsync(int id)
      => await _systemappsRepository.GetByIdAsync(id);

   public async Task<SystemApp> CreateAsync(SystemApp entity, string userId)
   {
      entity.SetCreated(userId);
      return await _systemappsRepository.AddAsync(entity);
   }

   public async Task UpdateAsync(SystemApp entity, string userId)
   {
      entity.SetUpdated(userId);
      await _systemappsRepository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(SystemApp entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _systemappsRepository.UpdateAsync(entity);
   }

}
