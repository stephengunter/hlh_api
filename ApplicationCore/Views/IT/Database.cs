using Infrastructure.Helpers;
using Infrastructure.Entities;
using Microsoft.Extensions.Hosting;
using Infrastructure.Views;

namespace ApplicationCore.Views.IT;
public class DatabaseViewModel : EntityBaseView, IBaseRecordView
{
   public string Title { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;
   public string Ps { get; set; } = string.Empty;
   public int ServerId { get; set; }
   public ServerViewModel? Server { get; set; }

   public ICollection<DbBackupPlanViewModel> BackupPlans { get; set; } = new List<DbBackupPlanViewModel>();
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

