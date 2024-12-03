using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models.IT;
[Table("IT.DbBackupTasks")]
public class DbBackupTask : EntityBase
{
   public int PlanId { get; set; }

   [Required]
   public DbBackupPlan? Plan { get; set; }
   public string Ps { get; set; } = string.Empty;

   public int? TargetServerId { get; set; }
   public string TargetPath { get; set; } = string.Empty;

   public DateTime StartAt { get; set; }

   public bool Done { get; set; }
   public string Error { get; set; } = string.Empty;
}