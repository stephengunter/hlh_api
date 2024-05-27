using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models;

public class Category : EntityBase, IBaseCategory<Category>, IRemovable, ISortable
{
   public PostType PostType { get; set; } = PostType.None;
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;

   public Category? Parent { get; set; }

   public int? ParentId { get; set; }

   public bool IsRootItem => ParentId is null;

   public ICollection<Category>? SubItems { get; set; }
   [NotMapped]
   public ICollection<int>? SubIds { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }

   public bool Active => ISortableHelpers.IsActive(this);

   public void LoadSubItems(IEnumerable<IBaseCategory<Category>> categories) => BaseCategoriesHelpers.LoadSubItems(this, categories);

   public virtual ICollection<CategoryPost>? CategoryPosts { get; set; }
}



public class CategoryPost : EntityBase
{

   public int CategoryId { get; set; }
   [Required]
   public virtual Category? Category { get; set; }

   public int PostId { get; set; }

   public PostType PostType { get; set; }
}
