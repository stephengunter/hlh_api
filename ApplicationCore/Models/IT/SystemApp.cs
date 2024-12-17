using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Consts;


namespace ApplicationCore.Models.IT;

[Table("IT.SystemApps")]
public class SystemApp : EntityBase, IBaseCategory<SystemApp>, IBaseRecord, IRemovable, ISortable
{
   public string Title { get; set; } = string.Empty;
   public string Key { get; set; } = string.Empty;
   public string Type { get; set; } = string.Empty;
   public bool Centralized { get; set; }
   public int? ServerId { get; set; }
   [NotMapped]
   public Server? Server { get; set; }


   public virtual SystemApp? Parent { get; set; }
   public int? ParentId { get; set; }
   public bool IsRootItem => ParentId is null;

   public ICollection<SystemApp>? SubItems { get; set; }
   [NotMapped]
   public ICollection<int>? SubIds { get; set; }
   public virtual ICollection<SystemAppDatabase>? SystemAppDatabases { get; set; }

   public Importance Importance { get; set; }
   public string Ps { get; set; } = string.Empty;
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public void LoadSubItems(IEnumerable<IBaseCategory<SystemApp>> categories)
   {
      throw new NotImplementedException();
   }
}


public class SystemAppDatabase
{
   public int SystemAppId { get; set; }
   [Required]
   public virtual SystemApp? SystemApp { get; set; }

   public int DatabaseId { get; set; }

   [Required]
   public virtual Database? Database { get; set; }
}

