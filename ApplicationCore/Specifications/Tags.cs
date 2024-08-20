using Ardalis.Specification;
using ApplicationCore.Models;

namespace ApplicationCore.Specifications;
public class TagSpecification : Specification<Tag>
{
   public TagSpecification(string title, bool allmatch = false)
	{
      if(allmatch) Query.Where(item => item.Title.ToLower() == title.ToLower());
      else Query.Where(item => item.Title.ToLower().Contains(title.ToLower()));
   }

}

