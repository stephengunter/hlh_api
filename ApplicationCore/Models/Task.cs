using ApplicationCore.Consts;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models;

public class Tasks : EntityBase, IBaseCategory<Tasks>, IBaseRecord, IRemovable, ISortable
{
   public string UserId { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;

   public string? Content { get; set; }

   public DateTime? DeadLine { get; set; }

   public bool Done { get; set; }

   public Tasks? Parent { get; set; }

   public int? ParentId { get; set; }

   public bool IsRootItem => ParentId is null;

   public ICollection<Tasks>? SubItems { get; set; } = new List<Tasks>();
   [NotMapped]
   public ICollection<int>? SubIds { get; set; }

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public int Order { get; set; }

   public bool Active => ISortableHelpers.IsActive(this);

   public void LoadSubItems(IEnumerable<IBaseCategory<Tasks>> tasks) => BaseCategoriesHelpers.LoadSubItems(this, tasks);


   [NotMapped] 
   public virtual ICollection<Reference> References { get; set; } = new List<Reference>();


   public void LoadReferences(IEnumerable<Reference> references)
   {
      references = references.Where(x => x.PostType.ToLower() == PostTypes.Tasks.ToLower() && x.PostId == Id);
      this.References = references.HasItems() ? references.ToList() : new List<Reference>();
   }
}