using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;
public class Event : EntityBase, IBasePost, IBaseContract, IBaseRecord, IRemovable
{
   public string UserId { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string? Content { get; set; }
   public DateTime? StartDate { get; set; }
   public DateTime? EndDate { get; set; }

   public virtual ICollection<LocationEvent>? LocationEvents { get; set; }

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public virtual ICollection<EventCalendar>? EventCalendars { get; set; }

   public ContractStatus Status => BaseContractHelpers.GetStatus(this);


   [NotMapped]
   public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

   public void LoadAttachments(IEnumerable<Attachment> attachments)
   {
      attachments = attachments.Where(x => x.PostType == PostType.Event && x.PostId == Id);
      this.Attachments = attachments.HasItems() ? attachments.ToList() : new List<Attachment>();
   }
}


public class LocationEvent
{
   public int EventId { get; set; }

   [Required]
   public virtual Event? Event { get; set; }

   public int LocationId { get; set; }
   [Required]
   public virtual Location? Location { get; set; }
}

