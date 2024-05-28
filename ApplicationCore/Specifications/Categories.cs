using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class RootCategoriesSpecification : Specification<Category>
{
   public RootCategoriesSpecification()
   {
      Query.Where(item => item.IsRootItem && !item.Removed);
   }
   public RootCategoriesSpecification(PostType type)
   {
      Query.Where(item => item.IsRootItem && !item.Removed && item.PostType == type);
   }
}
public class CategoriesSpecification : Specification<Category>
{
	public CategoriesSpecification()
	{
		Query.Where(item => !item.Removed);
	}

   public CategoriesSpecification(PostType type, string key)
	{
		Query.Where(item => !item.Removed && item.PostType == type && item.Key == key);
	}
   public CategoriesSpecification(PostType type, IList<string> keys)
   {
      Query.Where(item => !item.Removed && item.PostType == type && keys.Contains(item.Key.ToLower()));
   }

}
