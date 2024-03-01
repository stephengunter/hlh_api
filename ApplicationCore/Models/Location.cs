using ApplicationCore.Consts;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models;

public class Location : EntityBase, IBaseCategory<Location>, IRemovable, ISortable
{
   public string Key { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;

   public virtual Location? Parent { get; set; }
   
   public int? ParentId { get; set; }

   public bool IsRootItem => ParentId is null;

   public ICollection<Location>? SubItems { get; set; }
   [NotMapped]
   public ICollection<int>? SubIds { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }

   public bool Active => ISortableHelpers.IsActive(this);

   public void LoadSubItems(IEnumerable<IBaseCategory<Location>> locations) => BaseCategoriesHelpers.LoadSubItems(this, locations);
}


