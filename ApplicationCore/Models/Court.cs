using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;

public class Court : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Title { get; set; } = string.Empty;

   public string Key { get; set; } = string.Empty;

   public string? Ps { get; set; }

   public string? Utils { get; set; } //遠距詢問FD, 雙向詢問DW, U會議詢問UM

   public int LocationId { get; set; }

   [Required]
   public virtual Location? Location { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);
}


