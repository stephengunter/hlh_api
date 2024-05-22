using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace ApplicationCore.Models;

public class Job : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public int JobTitleId { get; set; }
   public virtual required JobTitle JobTitle { get; set; }

   public int DepartmentId { get; set; }

   public virtual required Department Department { get; set; }
   
   public JobRole Role { get; set; } 

   public string? Tel { get; set; }

   public string? SubTel { get; set; }

   public string? Ps { get; set; }

   public virtual ICollection<JobUserProfiles>? JobUserProfiles { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);
}

public class JobTitle : EntityBase, ISortable
{
   public string Title { get; set; } = string.Empty;
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);
   public virtual ICollection<Job>? Jobs { get; set; }
}

public enum JobRole
{ 
   Normal = 0, // 0: 一般
   Vice = 1, // 1: 副主管
   Chief = 2  // 2: 主管
}



