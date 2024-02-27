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

public interface IProfilesService
{
   Task<IEnumerable<Profiles>> FetchAsync();

   Task<Profiles?> FindAsync(User user);
   Task<Profiles> CreateAsync(Profiles Profiles);
	Task UpdateAsync(Profiles Profiles);
   Task DeleteAsync(Profiles Profiles);
}

public class ProfilessService : IProfilesService
{
	private readonly IDefaultRepository<Profiles> _profilesRepository;

	public ProfilessService(IDefaultRepository<Profiles> profilesRepository)
	{
      _profilesRepository = profilesRepository;
	}
   public async Task<IEnumerable<Profiles>> FetchAsync()
      => await _profilesRepository.ListAsync();

   public async Task<Profiles?> FindAsync(User user)
      => await _profilesRepository.FirstOrDefaultAsync(new ProfilesSpecification(user));

   public async Task<Profiles> CreateAsync(Profiles Profiles)
		=> await _profilesRepository.AddAsync(Profiles);

	public async Task UpdateAsync(Profiles Profiles)
		=> await _profilesRepository.UpdateAsync(Profiles);

   public async Task DeleteAsync(Profiles Profiles)
      => await _profilesRepository.DeleteAsync(Profiles);

}
