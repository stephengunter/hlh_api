using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.Keyin;

[Table("Keyin.BranchRecords")]
public class BranchRecord : EntityBase
{
   public int Year { get; set; }
   public int Month { get; set; }
   public int BranchId { get; set; }
   public string AbsentRate { get; set; } = string.Empty;
   public int Score { get; set; }
   public int Rank { get; set; }
   public string Diff { get; set; } = string.Empty;

   [NotMapped]
   public Branch? Branch { get; set; }
}

[Table("Keyin.PersonRecords")]
public class PersonRecord : EntityBase
{
   public int Year { get; set; }
   public int Month { get; set; }

   public string Unit { get; set; } = string.Empty;
   public double CorrectRate { get; set; }
   public int PersonId { get; set; }
   public int Score { get; set; }
   public string Diff { get; set; } = string.Empty;

   [NotMapped]
   public KeyinPerson? Person { get; set; }
}


