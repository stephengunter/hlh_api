using ApplicationCore.Consts;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models;

public class Item : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string PostType { get; set; } = String.Empty;
   public int PostId { get; set; }

   public string Title { get; set; } = String.Empty;
   public bool Done { get; set; }

   
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);
   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   


   [NotMapped] 
   public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();


   public void LoadAttachments(IEnumerable<Attachment> attachments)
   {
      attachments = attachments.Where(x => x.PostType.ToLower() == PostTypes.Item.ToLower() && x.PostId == Id);
      this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<Attachment>();
   }
}