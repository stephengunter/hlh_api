using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models.IT;

[Table("IT.Servers")]
public class Server : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public int HostId { get; set; }

   [Required]
   public virtual Host? Host { get; set; }
   public string Type { get; set; } = string.Empty;
   public string Provider { get; set; } = string.Empty;
   public string Root { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;
   public string Key { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   [NotMapped]
   public virtual ICollection<CredentialInfo> CredentialInfoes { get; set; } = new List<CredentialInfo>();

   
   public void LoadCredentialInfoes(IEnumerable<CredentialInfo> credentialInfos)
   {
      credentialInfos = credentialInfos.Where(x => x.EntityType == nameof(Server) && x.EntityId == Id);
      this.CredentialInfoes = credentialInfos.HasItems() ? credentialInfos.ToList() : new List<CredentialInfo>();
   }
}

