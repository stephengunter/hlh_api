using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class ItemsSpecification : Specification<Item>
{
   public ItemsSpecification()
   {
      Query.Where(item => !item.Removed);
   }
   public ItemsSpecification(string postType, int postId)
   {
      Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower() && item.PostId == postId);
   }
   public ItemsSpecification(IList<int> ids)
	{
		Query.Where(item => !item.Removed && ids.Contains(item.Id));
	}
}
