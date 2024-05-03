using ApplicationCore.Consts;
using ApplicationCore.Models;
using ApplicationCore.Models.Auth;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Auth;
public class AuthTokenSpecification : Specification<AuthToken>
{
   public AuthTokenSpecification(string username, AuthProvider provider)
   {
      Query.Where(item => item.UserName.ToLower() == username.ToLower() && item.Provider == provider);
   }
}
