using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;
public class Tag : EntityBase, IBaseRecord
{
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public virtual ICollection<TagPost>? TagPosts { get; set; }
}

public class TagPost : EntityBase
{
   public int TagId { get; set; }
   [Required]
   public virtual Tag? Tag { get; set; }

   public string EntityId { get; set; } = String.Empty;

   public string EntityType { get; set; } = String.Empty;
}
