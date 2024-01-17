using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class DepartmentSpecification : Specification<Department>
{
	public DepartmentSpecification()
	{
		Query.Where(item => !item.Removed);
	}

}

