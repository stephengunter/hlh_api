using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.IT;
public class DbBackupPlanViewModel : EntityBaseView, IBaseRecordView
{
   public string Title { get; set; } = String.Empty;
   public string Ps { get; set; } = string.Empty;
   public string Type { get; set; } = string.Empty;
   public int StartTime { get; set; }
   public int MinutesInterval { get; set; }
   public int DatabaseId { get; set; }
   public virtual DatabaseViewModel? Database { get; set; }
   public virtual ServerViewModel? TargetServer { get; set; }
   public int? TargetServerId { get; set; }
   public string TargetPath { get; set; } = string.Empty;

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateTimeString();
   public string LastUpdatedText => LastUpdated.ToDateTimeString();
}