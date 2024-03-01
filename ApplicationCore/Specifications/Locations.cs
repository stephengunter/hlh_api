using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class LocationSpecification : Specification<Location>
{
	public LocationSpecification()
	{
		Query.Where(item => !item.Removed);
	}
   public LocationSpecification(string key)
   {
      Query.Where(item => !item.Removed).Where(item => item.Key == key);
   }
   public LocationSpecification(Location? parent)
   {
      if(parent is null) Query.Where(item => !item.Removed).Where(item => item.ParentId == null);
      else Query.Where(item => !item.Removed).Where(item => item.ParentId == parent.Id);
   }
   public LocationSpecification(IEnumerable<int> ids)
   {
      Query.Where(item => !item.Removed).Where(item => ids.Contains(item.Id));
   }
}

