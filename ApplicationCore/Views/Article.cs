using ApplicationCore.Models;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Views;

public class ArticleViewModel : EntityBaseView, IBaseRecordView
{
   public string UserId { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;
   public string? Content { get; set; } = String.Empty;
   public string? Summary { get; set; } = String.Empty;
   public string? Cover { get; set; } = String.Empty;

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }

   public virtual ICollection<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();

   public DateTime CreatedAt { get; set; }
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();
}

