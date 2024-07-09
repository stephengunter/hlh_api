using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using ApplicationCore.Consts;

namespace ApplicationCore.Models;

public class Reference : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string PostType { get; set; } = String.Empty;
   public int PostId { get; set; }

   public int? AttachmentId { get; set; }
   public string Title { get; set; } = String.Empty;
   public string Url { get; set; } = String.Empty;
   public int Order { get; set; }

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   [NotMapped]
   public virtual Attachment? Attachment { get; set; }
   

   public void LoadAttachment(IEnumerable<Attachment> attachments)
   {
      if (this.AttachmentId.HasValue)
      {
         this.Attachment = attachments.FirstOrDefault(x => x.PostType.ToLower() == PostTypes.Reference && x.PostId == Id);
      }
      
   }
}

