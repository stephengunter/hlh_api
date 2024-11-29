using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface IHostService
{
   Task<IEnumerable<Host>> FetchAsync();
   Task<Host?> GetByIdAsync(int id);
   Task<Host> CreateAsync(Host entity, string userId);
   Task UpdateAsync(Host entity, string userId);
   Task RemoveAsync(Host entity, string userId);
}

public class HostService : IHostService
{
	private readonly IDefaultRepository<Host> _hostsRepository;

	public HostService(IDefaultRepository<Host> hostRepository)
	{
      _hostsRepository = hostRepository;
	}
   public async Task<IEnumerable<Host>> FetchAsync()
       => await _hostsRepository.ListAsync(new HostSpecification());


   public async Task<Host?> GetByIdAsync(int id)
      => await _hostsRepository.FirstOrDefaultAsync(new HostSpecification(id));

   public async Task<Host> CreateAsync(Host entity, string userId)
   {
      entity.SetCreated(userId);
      return await _hostsRepository.AddAsync(entity);
   }

   public async Task UpdateAsync(Host entity, string userId)
   {
      entity.SetUpdated(userId);
      await _hostsRepository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(Host entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _hostsRepository.UpdateAsync(entity);
   }

}
