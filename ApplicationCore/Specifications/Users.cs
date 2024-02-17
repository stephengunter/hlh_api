using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class UsersSpecification : Specification<User>
{
	public UsersSpecification()
	{
      Query.Include(u => u.Profiles).Include(u => u.UserRoles);
   }
   public UsersSpecification(string id)
   {
      Query.Include(u => u.Profiles).Include(u => u.UserRoles).Where(user => user.Id == id);
   }
}

