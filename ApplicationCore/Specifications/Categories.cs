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

   public CategoriesSpecification(string key)
	{
		Query.Where(item => !item.Removed && item.Key == key);
	}
   public CategoriesSpecification(IList<string> keys, PostType type)
   {
      Query.Where(item => !item.Removed && item.PostType == type && keys.Contains(item.Key.ToLower()));
   }

}
