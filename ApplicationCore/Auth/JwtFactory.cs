using ApplicationCore.Auth;
using ApplicationCore.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using ApplicationCore.Consts;
using System.Security.Principal;
using ApplicationCore.Models.Auth;
using ApplicationCore.Views.Jud;


namespace ApplicationCore.Auth;
public interface IJwtFactory
{
   Task<AccessToken> GenerateEncodedTokenAsync(User user, IList<string>? roles, OAuth? oAuth = null);
   Task<AccessToken> GenerateEncodedTokenAsync(User user, IList<string>? roles, IList<AdUserViewModel> adUsers);
}


public class JwtFactory : IJwtFactory
{
   private readonly IJwtTokenHandler _jwtTokenHandler;
   private readonly JwtIssuerOptions _jwtOptions;

   internal JwtFactory(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions)
   {
      _jwtTokenHandler = jwtTokenHandler;
      _jwtOptions = jwtOptions.Value;
      ThrowIfInvalidOptions(_jwtOptions);
   }
   public async Task<AccessToken> GenerateEncodedTokenAsync(User user, IList<string>? roles, IList<AdUserViewModel> adUsers)
   {
      var dpts = adUsers.Select(ad => ad.dpt).JoinToString();
      var claims = new List<Claim>()
      {
         new Claim(JwtClaimIdentifiers.Sub, user.UserName!),
         new Claim(JwtClaimIdentifiers.Roles, roles.JoinToString()),
         new Claim(JwtClaimIdentifiers.Id, user.Id),
         new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
         new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
         new Claim(JwtClaimIdentifiers.Name, user.Name),
         new Claim("ad_dpts", dpts)
      };


      // Create the JWT security token and encode it.
      var jwt = new JwtSecurityToken(
          _jwtOptions.Issuer,
          _jwtOptions.Audience,
          claims,
          _jwtOptions.NotBefore,
          _jwtOptions.Expiration,
          _jwtOptions.SigningCredentials);

      return new AccessToken(_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);

   }

   public async Task<AccessToken> GenerateEncodedTokenAsync(User user, IList<string>? roles, OAuth? oAuth = null)
   {
      var claims = new List<Claim>()
      {
         new Claim(JwtClaimIdentifiers.Sub, user.UserName!),
         new Claim(JwtClaimIdentifiers.Roles, roles.JoinToString()),
         new Claim(JwtClaimIdentifiers.Id, user.Id),
         new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
         new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64)
      };

      if (oAuth != null)
      {
         claims.Add(new Claim(JwtClaimIdentifiers.Provider, oAuth.Provider.ToString()));
         claims.Add(new Claim(JwtClaimIdentifiers.Picture, oAuth.PictureUrl.GetString()));
         claims.Add(new Claim(JwtClaimIdentifiers.Name, oAuth.GivenName.GetString()));
      }
      else
      {
         claims.Add(new Claim(JwtClaimIdentifiers.Name, user.Name));
      }

      // Create the JWT security token and encode it.
      var jwt = new JwtSecurityToken(
          _jwtOptions.Issuer,
          _jwtOptions.Audience,
          claims,
          _jwtOptions.NotBefore,
          _jwtOptions.Expiration,
          _jwtOptions.SigningCredentials);

      return new AccessToken(_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);

   }
   private static long ToUnixEpochDate(DateTime date)
     => (long)Math.Round((date.ToUniversalTime() -
                          new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                         .TotalSeconds);

   private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
   {
      if (options == null) throw new ArgumentNullException(nameof(options));

      if (options.ValidFor <= TimeSpan.Zero)
      {
         throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
      }

      if (options.SigningCredentials == null)
      {
         throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
      }

      if (options.JtiGenerator == null)
      {
         throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
      }
   }
}