using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class EventSpecification : Specification<Event>
{
   public EventSpecification(IList<int> ids)
	{
		Query.Where(item => !item.Removed && ids.Contains(item.Id));
	}
}

