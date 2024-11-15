using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.CredentialInfoes")]
public class CredentialInfo : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string EntityType { get; set; } = String.Empty;
   public int EntityId { get; set; }
   public string Username { get; set; } = String.Empty;
   public string Password { get; set; } = string.Empty;

   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
}

