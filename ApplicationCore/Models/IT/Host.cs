using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Helpers;
using Infrastructure.Entities;

namespace ApplicationCore.Models.IT;

[Table("IT.Hosts")]
public class Host : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string IP { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
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
      credentialInfos = credentialInfos.Where(x => x.EntityType == nameof(Host) && x.EntityId == Id);
      this.CredentialInfoes = credentialInfos.HasItems() ? credentialInfos.ToList() : new List<CredentialInfo>();
   }
}

