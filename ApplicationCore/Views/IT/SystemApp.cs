using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Views;


namespace ApplicationCore.Views.IT;
public class SystemAppViewModel : EntityBaseView, IBaseRecordView
{
   public string Title { get; set; } = string.Empty;
   public string Key { get; set; } = string.Empty;
   public string CredentialInfoId { get; set; } = string.Empty;
   public string? HostId { get; set; }
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

