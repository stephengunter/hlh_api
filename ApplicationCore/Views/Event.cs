using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;
public class EventViewModel : EntityBaseView, IBasePostView, IBaseRecordView, IBaseContractView
{
   public string Title { get; set; } = String.Empty;
   public string? Content { get; set; }
   public DateTime? StartDate { get; set; }
   public DateTime? EndDate { get; set; }

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public int? CategoryId { get; set; }
   public string? CategoryTitle { get; set; }

   public int Status { get; set; }
   public string StatusText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();
}

