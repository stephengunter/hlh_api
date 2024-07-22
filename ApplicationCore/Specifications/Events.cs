using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class EventSpecification : Specification<Event>
{
   public EventSpecification(IList<int> ids)
	{
		Query.Where(e => !e.Removed && ids.Contains(e.Id));
	}
	public EventSpecification(DateTime start, DateTime end)
	{
		Query.Where(e => !e.Removed && e.StartDate.HasValue && e.EndDate.HasValue && e.StartDate.Value <= end && e.EndDate.Value >= start);
	}
}

