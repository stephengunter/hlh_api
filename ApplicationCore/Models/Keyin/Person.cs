using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.Keyin;


[Table("Keyin.Persons")]
public class KeyinPerson : EntityBase
{
   public string Account { get; set; } = string.Empty;
   public string Unit { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;

   public bool AllPass { get; set; }
   public int HighRun { get; set; }

   public DateTime? LeaveAt { get; set; }
   public bool IsActive(int year, int month)
   {
      if(LeaveAt is null) return true;
      if (LeaveAt.Value.Year > year) return true;
      else if (LeaveAt.Value.Year < year) return false;
      else 
      { 
         return LeaveAt.Value.Month > month;
      }
   }
}

