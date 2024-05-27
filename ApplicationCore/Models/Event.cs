using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class Event : EntityBase, IBasePost, IBaseContract, IBaseRecord, IRemovable
{
   public string UserId { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;
   public string? Content { get; set; }
   public DateTime? StartDate { get; set; }
   public DateTime? EndDate { get; set; }

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   //public int? CategoryId { get; set; }
   //public virtual Category? Category { get; set; }

   [NotMapped]
   public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

   public ContractStatus Status => BaseContractHelpers.GetStatus(this);

   public void LoadAttachments(IEnumerable<Attachment> attachments)
   {
      attachments = attachments.Where(x => x.PostType == PostType.Event && x.PostId == Id);
      this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<Attachment>();
   }
}

