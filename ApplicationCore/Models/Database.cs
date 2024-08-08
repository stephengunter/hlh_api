using Infrastructure.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models;

public class Database : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Provider { get; set; } = string.Empty;
   public string Host { get; set; } = string.Empty;
   public string Username { get; set; } = string.Empty;
   public string Password { get; set; } = string.Empty;
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
}

