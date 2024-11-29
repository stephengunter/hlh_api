using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using Infrastructure.Views;
using ApplicationCore.Models.IT;

namespace ApplicationCore.Views.IT;
public class ServerViewModel : EntityBaseView, IBaseRecordView
{
   public int HostId { get; set; }
   public HostViewModel? Host { get; set; }
   public string Type { get; set; } = String.Empty;
   public string Provider { get; set; } = string.Empty;
   public string Root { get; set; } = string.Empty;
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Ps { get; set; } = string.Empty;
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }
   public string Name
   {
      get 
      {
         if (String.IsNullOrEmpty(Title))
         {
            if (Host != null) return Host.Name;
         }
         return Title;
      }
   }
  

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateTimeString();
   public string LastUpdatedText => LastUpdated.ToDateTimeString(); 
   public ICollection<CredentialInfoViewModel> CredentialInfoes { get; set; } = new List<CredentialInfoViewModel>();


}

