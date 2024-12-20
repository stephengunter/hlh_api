using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using ApplicationCore.Attributes;

namespace ApplicationCore.Models.IT;

[Table("IT.Hosts")]
public class Host : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string IP { get; set; } = String.Empty;
   [Editor("¦WºÙ", Enable = true)]
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;

   [Editor("³Æµù", Enable = true)]
   public string Ps { get; set; } = string.Empty;
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);
   public virtual ICollection<Server> Servers { get; set; } = new List<Server>();

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   [NotMapped]
   public virtual ICollection<CredentialInfo> CredentialInfoes { get; set; } = new List<CredentialInfo>();

   
   public void LoadCredentialInfoes(IEnumerable<CredentialInfo> credentialInfos)
   {
      credentialInfos = credentialInfos.Where(x => x.EntityType == nameof(Host) && x.EntityId == Id);
      this.CredentialInfoes = credentialInfos.HasItems() ? credentialInfos.ToList() : new List<CredentialInfo>();
   }
}

