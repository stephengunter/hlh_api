using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Helpers;

namespace ApplicationCore.Models.Cars;


[Table("Cars.Rents")]
public class CarRent : EntityBase, IBaseContract, IBaseRecord, IRemovable
{
   public int CarId { get; set; }

   public virtual required Car Car { get; set; }

   [ForeignKey("UserProfiles")]
   public string? UserId { get; set; }

   public virtual required Profiles UserProfiles { get; set; }

   public string? DriverId { get; set; }
   [NotMapped]
   public Profiles? DriverProfiles { get; set; }

   public string? PS { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   public bool Removed { get; set; }

   public DateTime? StartDate { get; set; }
   public DateTime? EndDate { get; set; }

   public ContractStatus Status => BaseContractHelpers.GetStatus(this);
}



