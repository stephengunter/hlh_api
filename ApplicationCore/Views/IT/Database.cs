using Infrastructure.Helpers;
using Infrastructure.Entities;
using Microsoft.Extensions.Hosting;
using Infrastructure.Views;

namespace ApplicationCore.Views.IT;
public class DatabaseViewModel : EntityBaseView, IBaseRecordView
{
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Provider { get; set; } = string.Empty;
   public int HostId { get; set; }
   

   public string CredentialInfoId { get; set; } = string.Empty;
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

