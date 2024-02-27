using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models;

public class Department : EntityBase, IBaseCategory<Department>, IBaseRecord, IRemovable, ISortable
{
   public string Key { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;

   public virtual Department? Parent { get; set; }
   
   public int? ParentId { get; set; }

   public bool IsRootItem => ParentId is null;

   public ICollection<Department>? SubItems { get; set; }
   [NotMapped]
   public ICollection<int>? SubIds { get; set; }
   
   public virtual ICollection<Job>? Jobs { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }

   public bool Active => ISortableHelpers.IsActive(this);

   public void LoadSubItems(IEnumerable<IBaseCategory<Department>> departments) => BaseCategoriesHelpers.LoadSubItems(this, departments);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

}

