using ApplicationCore.Models;
using Ardalis.Specification;
using CategoryPost = ApplicationCore.Models.CategoryPost;

namespace ApplicationCore.Specifications;
public class CategoryPostsSpecification : Specification<CategoryPost>
{
	public CategoryPostsSpecification(Category category, string postType)
	{
		Query.Where(item => item.CategoryId == category.Id && item.PostType.ToLower() == postType.ToLower());
	}
}
