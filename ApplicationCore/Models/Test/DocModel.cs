using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;
public class UnitPerson : EntityBase
{
   public string Unit { get; set; } = string.Empty;
   public string Person { get; set; } = string.Empty;
   public int Flag { get; set; }
   public int Saves { get; set; }
   public DateTime? LastUpdated { get; set; }
   public string Ip { get; set; } = string.Empty;
   public bool Confirmed { get; set; }
   public string LastUpdatedText => LastUpdated.ToDateTimeString();
}
public class DocModel : EntityBase
{
   public string Num { get; set; } = string.Empty;
   public string Old_Num { get; set; } = string.Empty;
   public string Old_CNum { get; set; } = string.Empty;
   public string Unit { get; set; } = string.Empty;
   public string Date { get; set; } = string.Empty;
   public string Person { get; set; } = string.Empty;
   public string Result { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;

   public string? Ps { get; set; }
   public string? NewPersonName { get; set; }
   public string? NewPersonId { get; set; }

   public int Flag { get; set; }
   public int Year { get; set; }
   public int Keep { get; set; }
   public int Modified { get; set; }

   public int DateNumber => Date.ToDateNumber();
}


