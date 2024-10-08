using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class DepartmentSpecification : Specification<Department>
{
	public DepartmentSpecification()
	{
		Query.Where(item => !item.Removed);
	}
   public DepartmentSpecification(string key)
   {
      Query.Where(item => !item.Removed).Where(item => item.Key == key);
   }
   public DepartmentSpecification(Department? parent)
   {
      if(parent is null) Query.Where(item => !item.Removed).Where(item => item.ParentId == null);
      else Query.Where(item => !item.Removed).Where(item => item.ParentId == parent.Id);
   }
   public DepartmentSpecification(IEnumerable<int> ids)
   {
      Query.Where(item => !item.Removed).Where(item => ids.Contains(item.Id));
   }
}

public class DepartmentTitleSpecification : Specification<Department>
{
   public DepartmentTitleSpecification(string title)
   {
      Query.Where(item => !item.Removed).Where(item => item.Title == title);
   }
}

