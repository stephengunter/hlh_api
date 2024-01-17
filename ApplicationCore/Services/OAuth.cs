using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ApplicationCore.Consts;

namespace ApplicationCore.Services;

public interface IOAuthService
{
	Task<OAuth> CreateAsync(OAuth oAuth);
	Task UpdateAsync(OAuth oAuth);

	Task<OAuth?> FindByProviderAsync(User user, OAuthProvider provider);
}

public class OAuthService : IOAuthService
{
	private readonly IDefaultRepository<OAuth> _oAuthRepository;

	public OAuthService(IDefaultRepository<OAuth> oAuthRepository)
	{		
		_oAuthRepository = oAuthRepository;
	}
	

	public async Task<OAuth?> FindByProviderAsync(User user, OAuthProvider provider)
		=> await _oAuthRepository.FindByProviderAsync(user, provider);


	public async Task<OAuth> CreateAsync(OAuth oAuth)
		=> await _oAuthRepository.AddAsync(oAuth);

		public async Task UpdateAsync(OAuth oAuth)
		=> await _oAuthRepository.UpdateAsync(oAuth);

}
