using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ApplicationCore.Models.Auth;
using ApplicationCore.Views.Jud;

namespace ApplicationCore.Services.Auth;

public interface IJwtTokenService
{
    Task<AccessToken> CreateAccessTokenAsync(string ipAddress, User user, IList<string>? roles = null, OAuth? oAuth = null);
   Task<AccessToken> CreateAccessTokenAsync(string ipAddress, User user, IList<string>? roles, IList<AdUserViewModel> adUsers);
   Task<string> CreateRefreshTokenAsync(string ipAddress, User user);

    ClaimsPrincipal? ResolveClaimsFromToken(string accessToken);

    Task<bool> IsValidRefreshTokenAsync(string token, User user);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly AuthSettings _authSettings;

    private readonly IJwtFactory _jwtFactory;
    private readonly ITokenFactory _tokenFactory;
    private readonly IJwtTokenValidator _jwtTokenValidator;

    private readonly IDefaultRepository<RefreshToken> _refreshTokenRepository;

    public JwtTokenService(IOptions<AuthSettings> authSettings, IJwtFactory jwtFactory, ITokenFactory tokenFactory, IJwtTokenValidator jwtTokenValidator,
        IDefaultRepository<RefreshToken> refreshTokenRepository, IDefaultRepository<OAuth> oAuthRepository)
    {
        _authSettings = authSettings.Value;

        _jwtFactory = jwtFactory;
        _tokenFactory = tokenFactory;
        _jwtTokenValidator = jwtTokenValidator;

        _refreshTokenRepository = refreshTokenRepository;
    }

    int RefreshTokenDaysToExpire => _authSettings.RefreshTokenDaysToExpire < 1 ? 7 : _authSettings.RefreshTokenDaysToExpire;

    string SecretKey => _authSettings.SecurityKey;

    public async Task<AccessToken> CreateAccessTokenAsync(string ipAddress, User user, IList<string>? roles = null, OAuth? oAuth = null)
        => await _jwtFactory.GenerateEncodedTokenAsync(user, roles, oAuth);

   public async Task<AccessToken> CreateAccessTokenAsync(string ipAddress, User user, IList<string>? roles, IList<AdUserViewModel> adUsers)
      => await _jwtFactory.GenerateEncodedTokenAsync(user, roles, adUsers);
   public async Task<string> CreateRefreshTokenAsync(string ipAddress, User user)
    {
        var refreshToken = _tokenFactory.GenerateToken();
        await SetRefreshTokenAsync(ipAddress, user, refreshToken);
        return refreshToken;
    }

    public ClaimsPrincipal? ResolveClaimsFromToken(string accessToken)
        => _jwtTokenValidator.GetPrincipalFromToken(accessToken, SecretKey);


    public string GetUserIdFromToken(string accessToken)
    {
        var cp = _jwtTokenValidator.GetPrincipalFromToken(accessToken, SecretKey);
        if (cp == null) return "";

        return cp.Claims.First(c => c.Type == "id").Value;
    }

    public string GetOAuthProviderFromToken(string accessToken)
    {
        var cp = _jwtTokenValidator.GetPrincipalFromToken(accessToken, SecretKey);
        if (cp == null) return "";

        return cp.Claims.First(c => c.Type == "provider").Value;
    }

    public async Task<bool> IsValidRefreshTokenAsync(string token, User user)
    {
        var entity = await GetRefreshTokenAsync(user);
        if (entity == null) return false;

        return entity.Token == token && entity.Active;
    }

    async Task SetRefreshTokenAsync(string ipAddress, User user, string token)
    {
        var expires = DateTime.UtcNow.AddDays(RefreshTokenDaysToExpire);

        var exist = await GetRefreshTokenAsync(user);
        if (exist != null)
        {
            exist.Token = token;
            exist.Expires = expires;
            exist.RemoteIpAddress = ipAddress;
            exist.LastUpdated = DateTime.Now;
            await _refreshTokenRepository.UpdateAsync(exist);
        }
        else
        {
            var refreshToken = new RefreshToken
            {
                Token = token,
                Expires = expires,
                UserId = user.Id,
                RemoteIpAddress = ipAddress,
                LastUpdated = DateTime.Now
            };

            await _refreshTokenRepository.AddAsync(refreshToken);

        }

    }

    async Task<RefreshToken?> GetRefreshTokenAsync(User user)
        => await _refreshTokenRepository.FindAsync(user);

   
}
