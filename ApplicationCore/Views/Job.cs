using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class JobViewModel : EntityBaseView, IBaseRecordView
{
   public string Title { get; set; } = string.Empty;

   public int DepartmentId { get; set; }

   public DepartmentViewModel? Department { get; set; }

   public int Role { get; set; } // 0: �@��A1: �ƥD�ޡA2: �D�ޡA

   public string? Tel { get; set; }

   public string? SubTel { get; set; }

   public string? Ps { get; set; }

   public ICollection<JobUserProfilesViewModel>? JobUserProfiles { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }

   public DateTime CreatedAt { get; set; }
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   
   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();

   public string RoleText { get; set; } = string.Empty;



}

