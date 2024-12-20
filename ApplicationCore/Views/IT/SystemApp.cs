using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Views;
using ApplicationCore.Models.IT;


namespace ApplicationCore.Views.IT;
public class SystemAppViewModel : EntityBaseView, IBaseRecordView
{
   public string Title { get; set; } = string.Empty;
   public string Key { get; set; } = string.Empty;
   public string Type { get; set; } = string.Empty;
   public bool Centralized { get; set; }
   public SystemAppViewModel? Parent { get; set; }
   public int? ParentId { get; set; }
   public bool IsRootItem { get; set; }
   public ICollection<SystemAppViewModel>? SubItems { get; set; }
   public ICollection<int>? SubIds { get; set; }
   public int? ServerId { get; set; }
   public ServerViewModel? Server { get; set; }
   public string Importance { get; set; } = string.Empty;
   public string Url { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
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

