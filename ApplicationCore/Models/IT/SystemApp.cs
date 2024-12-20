using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Consts;
using ApplicationCore.Attributes;


namespace ApplicationCore.Models.IT;

[Table("IT.SystemApps")]
public class SystemApp : EntityBase, IBaseCategory<SystemApp>, IBaseRecord, IRemovable, ISortable
{
   [Editor("名稱", Enable = true)]
   public string Title { get; set; } = string.Empty;
   [Editor("Key", Enable = true)]
   public string Key { get; set; } = string.Empty;
   [Editor("類型", Enable = true)]
   public string Type { get; set; } = string.Empty;
   [Editor("集中化", Enable = true)]
   public bool Centralized { get; set; }
   [Editor("Server", Enable = true)]
   public int? ServerId { get; set; }
   [NotMapped]
   public Server? Server { get; set; }
   [Editor("網址", Enable = true)]
   public string Url { get; set; } = string.Empty;


   public virtual SystemApp? Parent { get; set; }
   [Editor("父系統", Enable = true)]
   public int? ParentId { get; set; }
   public bool IsRootItem => ParentId is null;

   public ICollection<SystemApp>? SubItems { get; set; }
   [NotMapped]
   public ICollection<int>? SubIds { get; set; }
   public virtual ICollection<SystemAppDatabase>? SystemAppDatabases { get; set; }

   [Editor("重要性", Enable = true)]
   public Importance Importance { get; set; }
   [Editor("備註", Enable = true)]
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

