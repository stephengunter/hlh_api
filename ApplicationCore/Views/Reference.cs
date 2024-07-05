using ApplicationCore.Models;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class ReferenceViewModel : EntityBaseView, IBaseRecordView
{
   public string PostType { get; set; } = String.Empty;
   public int PostId { get; set; }

   public int? AttachmentId { get; set; }
   public string Title { get; set; } = String.Empty;
   public string Url { get; set; } = String.Empty;
   public int Order { get; set; }

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();

   public AttachmentViewModel? Attachment { get; set; }
}

