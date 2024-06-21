using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class RootTaskSpecification : Specification<Tasks>
{
   public RootTaskSpecification()
   {
      Query.Where(item => item.IsRootItem && !item.Removed);
   }
}
public class TaskSpecification : Specification<Tasks>
{
	public TaskSpecification()
	{
		Query.Where(item => !item.Removed);
	}
   public TaskSpecification(Tasks parent)
	{
      Query.Where(item => !item.Removed && item.ParentId == parent.Id); 
		
	}

}
