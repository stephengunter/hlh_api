using ApplicationCore.Models;
using ApplicationCore.Models.Auth;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Auth;
public class RefreshTokensSpecification : Specification<RefreshToken>
{
   public RefreshTokensSpecification(User user)
   {
      Query.Where(item => item.UserId == user.Id);
   }
}
