using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class Article : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string UserId { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;
   public string? Content { get; set; }
   public string? Summary { get; set; }
   public string? Cover { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public int? CategoryId { get; set; }
   public virtual Category? Category { get; set; }

   [NotMapped]
   public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
   

   public void LoadAttachments(IEnumerable<Attachment> attachments)
   {
      attachments = attachments.Where(x => x.PostType == PostType.Article && x.PostId == Id);
      this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<Attachment>();
   }
}

