using Infrastructure.Helpers;
using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.Cars;

[Table("Cars.Cars")]
public class Car : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Title { get; set; } = String.Empty;
   public string Number { get; set; } = String.Empty;

   public int TypeId { get; set; }
   public virtual required CarType Type { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
}


[Table("Cars.Types")]
public class CarType : EntityBase, IBaseRecord, IRemovable, ISortable
{
   public string Title { get; set; } = String.Empty;
   public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
}

