using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class JobSpecification : Specification<Job>
{
	public JobSpecification()
	{
		Query.Include(item => item.JobTitle).Where(item => !item.Removed);
	}
   public JobSpecification(IEnumerable<Department> departments)
   {
      var depIds = departments.Select(d => d.Id);
      Query.Include(item => item.JobTitle).Where(item => !item.Removed && depIds.Contains(item.DepartmentId));
   }
   public JobSpecification(int id)
   {
      Query.Include(item => item.JobTitle).Include(item => item.Department).Where(user => user.Id == id);
   }
}

