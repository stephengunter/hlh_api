using ApplicationCore.DataAccess;
using ApplicationCore.Consts;
using ApplicationCore.Models.Auth;
using ApplicationCore.Specifications.Auth;

namespace ApplicationCore.Services.Auth;

public interface IAuthTokenService
{
   Task<AuthToken> CreateAsync(string username, AuthProvider provider, string ipAddress, string json, int minutes);
   Task<AuthToken?> CheckAsync(string token, string username, AuthProvider provider);
}

public class AuthTokenService : IAuthTokenService
{
    private readonly IDefaultRepository<AuthToken> _repository;

    public AuthTokenService(IDefaultRepository<AuthToken> repository)
    {
      _repository = repository;
    }

   public async Task<AuthToken> CreateAsync(string username, AuthProvider provider, string ipAddress, string json, int minutes)
   {
      var expires = DateTime.Now.AddMinutes(minutes > 0 ? minutes : 5);

      var exist = await FindAsync(username, provider);
      if (exist != null)
      {
         exist.Token = Guid.NewGuid().ToString();
         exist.Expires = expires;
         exist.RemoteIpAddress = ipAddress;
         exist.LastUpdated = DateTime.Now;
         exist.AdListJson = json;

         await _repository.UpdateAsync(exist);
         return exist;
      }
      else
      {
         var authToken = new AuthToken
         {
            UserName = username,
            Provider = provider,

            AdListJson = json,
            Token = Guid.NewGuid().ToString(),
            Expires = expires,
            RemoteIpAddress = ipAddress,
            LastUpdated = DateTime.Now
         };

         return await _repository.AddAsync(authToken);
      }
   }


   public async Task<AuthToken?> CheckAsync(string token, string username, AuthProvider provider)
   {
      var entity = await FindAsync(username, provider);
      if (entity == null) return null;

      if (entity.Token == token && entity.Active) return entity;
      return null;
   }

   async Task<AuthToken?> FindAsync(string username, AuthProvider provider)
      => await _repository.FirstOrDefaultAsync(new AuthTokenSpecification(username, provider));
}
