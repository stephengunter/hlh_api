using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class JobViewModel : EntityBaseView, IBaseRecordView
{
   public int JobTitleId { get; set; }
   public JobTitleViewModel? JobTitle  { get; set; }

   public int DepartmentId { get; set; }

   public DepartmentViewModel? Department { get; set; }

   public int Role { get; set; } // 0: 一般，1: 副主管，2: 主管，

   public string? Tel { get; set; }

   public string? SubTel { get; set; }

   public string? Ps { get; set; }

   public ICollection<JobUserProfilesViewModel>? JobUserProfiles { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }

   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   
   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();

   public string RoleText { get; set; } = string.Empty;



}

public class JobTitleViewModel : EntityBaseView
{
   public string Title { get; set; } = string.Empty;
   public int Order { get; set; }
   public bool Active { get; set; }
}
