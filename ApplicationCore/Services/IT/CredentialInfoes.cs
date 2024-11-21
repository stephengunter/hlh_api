using ApplicationCore.DataAccess;
using ApplicationCore.Models.IT;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface ICredentialInfoService
{
   Task<IEnumerable<CredentialInfo>> FetchAsync(Host host);
   Task<IEnumerable<CredentialInfo>> FetchAsync(Server server);
   Task<CredentialInfo?> GetByIdAsync(int id);
   Task<CredentialInfo> CreateAsync(CredentialInfo entity, string userId);
   Task UpdateAsync(CredentialInfo entity, string userId);
   Task RemoveAsync(CredentialInfo entity, string userId);
}

public class CredentialInfoService : ICredentialInfoService
{
	private readonly IDefaultRepository<CredentialInfo> _credentialInfoRepository;

	public CredentialInfoService(IDefaultRepository<CredentialInfo> credentialInfoRepository)
	{
      _credentialInfoRepository = credentialInfoRepository;
	}
   public async Task<IEnumerable<CredentialInfo>> FetchAsync(Host host)
       => await _credentialInfoRepository.ListAsync(new CredentialInfoSpecification(host));
   public async Task<IEnumerable<CredentialInfo>> FetchAsync(Server server)
       => await _credentialInfoRepository.ListAsync(new CredentialInfoSpecification(server));


   public async Task<CredentialInfo?> GetByIdAsync(int id)
      => await _credentialInfoRepository.GetByIdAsync(id);

   public async Task<CredentialInfo> CreateAsync(CredentialInfo entity, string userId)
   {
      entity.SetCreated(userId);
      return await _credentialInfoRepository.AddAsync(entity);
   }

   public async Task UpdateAsync(CredentialInfo entity, string userId)
   {
      entity.SetUpdated(userId);
      await _credentialInfoRepository.UpdateAsync(entity);
   }

   public async Task RemoveAsync(CredentialInfo entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _credentialInfoRepository.UpdateAsync(entity);
   }

}
