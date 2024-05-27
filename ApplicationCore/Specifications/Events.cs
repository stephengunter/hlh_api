using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class EventSpecification : Specification<Event>
{
	public EventSpecification()
	{
		Query.Where(item => !item.Removed);
	}
}

