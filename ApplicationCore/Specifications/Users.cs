using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class UsersSpecification : Specification<User>
{
	public UsersSpecification(bool includeRoles = false)
	{
      Query.Include(u => u.Profiles);
      if (includeRoles) Query.Include(u => u.UserRoles);
   }
   public UsersSpecification(string id, bool includeRoles = false)
   {
      Query.Include(u => u.Profiles);
      if (includeRoles) Query.Include(u => u.UserRoles);
      Query.Where(user => user.Id == id);
   }
   public UsersSpecification(IEnumerable<string> ids, bool includeRoles = false)
   {
      Query.Include(u => u.Profiles);
      if (includeRoles) Query.Include(u => u.UserRoles);
      Query.Where(user => ids.Contains(user.Id));
   }
}

