using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class ReferencesSpecification : Specification<Reference>
{
   public ReferencesSpecification()
   {
      Query.Where(item => !item.Removed);
   }
   public ReferencesSpecification(string postType, int postId)
   {
      Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower() && item.PostId == postId);
   }
   public ReferencesSpecification(IList<int> ids)
	{
		Query.Where(item => !item.Removed && ids.Contains(item.Id));
	}
}
