using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models.IT;
[Table("IT.DbBackupPlans")]
public class DbBackupPlan : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Title { get; set; } = String.Empty;
   public string Ps { get; set; } = string.Empty;
   public string Type { get; set; } = string.Empty;
   public int StartTime { get; set; }
   public int MinutesInterval { get; set; }
   public int DatabaseId { get; set; }

   public int? TargetServerId { get; set; }
   public string TargetPath { get; set; } = string.Empty;


   [Required]
   public virtual Database? Database { get; set; }
   public virtual Server? TargetServer { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
}