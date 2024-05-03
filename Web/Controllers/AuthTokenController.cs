using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using Web.Models;
using ApplicationCore.Exceptions;
using ApplicationCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Infrastructure.Helpers;
using ApplicationCore.Models.Auth;
using ApplicationCore.Services.Auth;
using System.Data;

namespace Web.Controllers;


[EnableCors("Open")]
public class AuthTokenController : BaseController
{
   private readonly AppSettings _appSettingss;
   private readonly Jud3Settings _jud3Settings;
	private readonly IAuthTokenService _authTokenService; 
   private readonly IUsersService _usersService;
   private readonly IJwtTokenService _jwtTokenService;

   public AuthTokenController(IOptions<AppSettings> appSettings, IOptions<Jud3Settings> jud3Settings, 
		IAuthTokenService authTokenService, IJwtTokenService jwtTokenService, IUsersService usersService)
	{
      _appSettingss = appSettings.Value;
      _jud3Settings = jud3Settings.Value;
      _authTokenService = authTokenService;
      _usersService = usersService;
      _jwtTokenService = jwtTokenService;
   }

	string AuthTokenLoginUrl(AuthToken model) => $"{_appSettingss.ClientUrl}/AuthToken/{model.UserName}/{model.Token}/{model.Provider.ToString().ToLower()}";
  
   [HttpPost]
	public async Task<ActionResult<AuthTokenResponse>> Create([FromBody] AuthTokenRequest model)
	{
      var provider = model.Provider.ToEnum<AuthProvider>(defaultValue: AuthProvider.Unknown);
		if (provider == AuthProvider.Unknown) throw new AuthTokenCreateFailedException($"provider: {model.Provider} 不存在");

		int minutes = 5;

      if (provider == AuthProvider.Jud3)
		{
			if (model.Key != _jud3Settings.Key) throw new AuthTokenCreateFailedException($"invalid key.");
			minutes = _jud3Settings.TokenValidMinutes;
      }
		else throw new AuthTokenCreateFailedException($"invalid key.");

      string json = model.AdListJson;
		string ip = RemoteIpAddress;
		var authToken = await _authTokenService.CreateAsync(model.UserName, provider, ip, json, minutes);

      string url = AuthTokenLoginUrl(authToken);
      return new AuthTokenResponse(model.UserName, authToken.Token, url);
	}

   [HttpPost("login")]
   public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthTokenLoginRequest model)
   {
      var provider = model.Provider.ToEnum<AuthProvider>(defaultValue: AuthProvider.Unknown);
      if (provider == AuthProvider.Unknown) throw new AuthTokenCreateFailedException($"provider: {model.Provider} 不存在");

      var valid = await _authTokenService.CheckAsync(model.Token, model.UserName, provider);

      if (!valid) throw new AuthTokenLoginFailedException($"provider: {model.Provider} , user: {model.UserName}");

      var user = await _usersService.FindByUsernameAsync(model.UserName);
      if (user == null)
      {
         user = await _usersService.CreateAsync(new User
         {
            UserName = model.UserName,
            Name = model.UserName,
            SecurityStamp = Guid.NewGuid().ToString(),
            Active = true
         });
      }

      var roles = await _usersService.GetRolesAsync(user);

      var accessToken = await _jwtTokenService.CreateAccessTokenAsync(RemoteIpAddress, user, roles);
      string refreshToken = await _jwtTokenService.CreateRefreshTokenAsync(RemoteIpAddress, user);

      return new AuthResponse(accessToken.Token, accessToken.ExpiresIn, refreshToken);
   }

}

