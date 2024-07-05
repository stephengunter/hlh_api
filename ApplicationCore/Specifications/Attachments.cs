using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class AttachmentsSpecification : Specification<Attachment>
{
	public AttachmentsSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public AttachmentsSpecification(ICollection<int> ids)
	{
		Query.Where(item => !item.Removed && ids.Contains(item.Id));
	}
}

public class AttachmentsByTypesSpecification : Specification<Attachment>
{
	public AttachmentsByTypesSpecification(string postType)
	{
		Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower());
	}
	public AttachmentsByTypesSpecification(string postType, int postId)
	{
		Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower() && item.PostId == postId);
	}
	public AttachmentsByTypesSpecification(string postType, IList<int> postIds)
	{
		Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower() && postIds.Contains(item.PostId));
	}
	public AttachmentsByTypesSpecification(ICollection<string> postTypes)
	{
		var types = postTypes.Select(x => x.ToLower());

      Query.Where(item => !item.Removed && types.Contains(item.PostType.ToLower()));
	}
}
