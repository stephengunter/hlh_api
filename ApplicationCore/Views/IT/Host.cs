using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using Infrastructure.Views;
using ApplicationCore.Models.IT;

namespace ApplicationCore.Views.IT;
public class HostViewModel : EntityBaseView, IBaseRecordView
{
   public string IP { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
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
   public ICollection<CredentialInfoViewModel> CredentialInfoes { get; set; } = new List<CredentialInfoViewModel>();


}

