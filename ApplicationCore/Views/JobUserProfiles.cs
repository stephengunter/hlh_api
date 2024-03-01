using ApplicationCore.Models;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class JobUserProfilesViewModel : EntityBaseView, IBaseRecordView, IBaseContractView
{
   public int JobId { get; set; }

   public JobViewModel? Job { get; set; }

   public string? UserId { get; set; }

   public ProfilesViewModel? UserProfiles { get; set; }

   public DateTime? StartDate { get; set; }
   public DateTime? EndDate { get; set; }

   public string? StartDateText { get; set; }
   public string? EndDateText { get; set; }

   public string? PS { get; set; }

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; }
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();

   public int Status { get; set; }
   public string StatusText { get; set; } = string.Empty;
}



