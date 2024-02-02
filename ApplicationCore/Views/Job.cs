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

   public int Role { get; set; } // 0: �@��A1: �ƥD�ޡA2: �D�ޡA

   public string? Tel { get; set; }

   public string? SubTel { get; set; }

   public string? Ps { get; set; }
}

