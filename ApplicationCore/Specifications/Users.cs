using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class UsersSpecification : Specification<User>
{
	public UsersSpecification()
	{
		Query.Include(u => u.Profile);
	}

}

