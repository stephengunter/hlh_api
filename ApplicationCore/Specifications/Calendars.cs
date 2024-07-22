using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class CalendarsSpecification : Specification<Calendar>
{
   public CalendarsSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public CalendarsSpecification(string key)
	{
		Query.Where(item => !item.Removed && key.ToLower() == item.Key.ToLower());
	}
	public CalendarsSpecification(ICollection<string> keys)
	{
		keys = keys.Select(item => item.ToLower()).ToList();
		Query.Where(item => !item.Removed && keys.Contains(item.Key.ToLower()));
	}
   public CalendarsSpecification(ICollection<int> ids)
   {
      Query.Where(item => !item.Removed && ids.Contains(item.Id));
   }
}

