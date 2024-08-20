using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;


namespace ApplicationCore.Models.IT;

[Table("IT.SystemApps")]
public class SystemApp : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Title { get; set; } = string.Empty;
   public string Key { get; set; } = string.Empty;
   public string CredentialInfoId { get; set; } = string.Empty;
   public string? HostId { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
}

