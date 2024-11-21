using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface IServerService
{
   Task<IEnumerable<Server>> FetchAsync(string include = "");
   Task<Server?> GetByIdAsync(int id, string include = "");
   Task<Server> CreateAsync(Server entity, string userId);
   Task UpdateAsync(Server entity, string userId);
   Task RemoveAsync(Server entity, string userId);
}

public class ServerService : IServerService
{
	private readonly IDefaultRepository<Server> _serversRepository;

	public ServerService(IDefaultRepository<Server> serversRepository)
	{
      _serversRepository = serversRepository;
	}
   public async Task<IEnumerable<Server>> FetchAsync(string include = "")
       => await _serversRepository.ListAsync(new ServerSpecification(include));


   public async Task<Server?> GetByIdAsync(int id, string include = "")
      => await _serversRepository.FirstOrDefaultAsync(new ServerSpecification(id, include));

   public async Task<Server> CreateAsync(Server entity, string userId)
   {
      entity.SetCreated(userId);
      return await _serversRepository.AddAsync(entity);
   }

   public async Task UpdateAsync(Server entity, string userId)
   {
      entity.SetUpdated(userId);
      await _serversRepository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(Server entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _serversRepository.UpdateAsync(entity);
   }

}
