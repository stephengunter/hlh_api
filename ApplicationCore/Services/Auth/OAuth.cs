using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using ApplicationCore.Consts;
using ApplicationCore.Models.Auth;

namespace ApplicationCore.Services.Auth;

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
