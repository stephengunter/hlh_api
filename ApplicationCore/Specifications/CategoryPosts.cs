using ApplicationCore.Migrations;
using ApplicationCore.Models;
using Ardalis.Specification;
using CategoryPost = ApplicationCore.Models.CategoryPost;

namespace ApplicationCore.Specifications;
public class CategoryPostsSpecification : Specification<CategoryPost>
{
	public CategoryPostsSpecification(Category category, PostType type)
	{
		Query.Where(item => item.CategoryId == category.Id && item.PostType == type);
	}
}
