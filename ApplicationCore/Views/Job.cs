using ApplicationCore.Models;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class JobViewModel : BaseRecordView
{
   public string Title { get; set; } = string.Empty;

   public int DepartmentId { get; set; }

   public DepartmentViewModel? Department { get; set; }

   public string? UserId { get; set; }

   public virtual UserViewModel? User { get; set; }

   public int Role { get; set; } // 0: 一般，1: 副主管，2: 主管，

   public string? Tel { get; set; }

   public string? SubTel { get; set; }

   public string? Ps { get; set; }
}

