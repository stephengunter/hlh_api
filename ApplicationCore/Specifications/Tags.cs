using Ardalis.Specification;
using ApplicationCore.Models;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class TagSpecification : Specification<Tag>
{
	public TagSpecification()
	{
		Query.Where(item => !item.Removed);
	}
   public TagSpecification(string title)
	{
		Query.Where(item => !item.Removed && item.Title.EqualTo(title));
	}

}

