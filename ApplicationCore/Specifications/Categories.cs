using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class RootCategoriesSpecification : Specification<Category>
{
   public RootCategoriesSpecification()
   {
      Query.Where(item => item.IsRootItem && !item.Removed);
   }
   public RootCategoriesSpecification(string postType)
   {
      Query.Where(item => item.IsRootItem && !item.Removed && item.PostType.ToLower() == postType.ToLower());
   }
}
public class CategoriesSpecification : Specification<Category>
{
	public CategoriesSpecification()
	{
		Query.Where(item => !item.Removed);
	}

   public CategoriesSpecification(string postType, string key)
	{
		Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower() && item.Key == key);
	}
   public CategoriesSpecification(string postType, IList<string> keys)
   {
      Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower() && keys.Contains(item.Key.ToLower()));
   }
   public CategoriesSpecification(string postType, string key, int parentId)
	{
      if(parentId > 0) Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower() && item.Key == key && item.ParentId == parentId); 
		else Query.Where(item => !item.Removed && item.PostType.ToLower() == postType.ToLower() && item.Key == key && (item.ParentId == null || item.ParentId == 0)); 
	}
   public CategoriesSpecification(Category parent)
	{
      Query.Where(item => !item.Removed && item.ParentId == parent.Id); 
		
	}

}
