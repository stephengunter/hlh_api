using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models;

public class Job : BaseRecord
{
   public string Title { get; set; } = string.Empty;

   public int DepartmentId { get; set; }

   public virtual required Department Department { get; set; }

   public string? UserId { get; set; }

   public virtual User? User { get; set; }

   public int Role { get; set; } // 0: �@��A1: �ƥD�ޡA2: �D�ޡA

   public string? Tel { get; set; }

   public string? SubTel { get; set; }

   public string? Ps { get; set; }

}



