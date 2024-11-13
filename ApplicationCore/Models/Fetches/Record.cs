using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.Fetches;

[Table("Fetches.Records")]
public class FetchesRecord : EntityBase
{
   public int SystemId { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Identifier { get; set; } = string.Empty;
   public string CourtType { get; set; } = string.Empty;
   public string CaseNumber { get; set; } = string.Empty;
   public int Year { get; set; }
   public int Month { get; set; }
   public int Day { get; set; }
   public string Time { get; set; } = string.Empty;
   public string QueryKey { get; set; } = string.Empty;
   public string IP { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
}

[Table("Fetches.Systems")]
public class FetchesSystem : EntityBase
{
   public string Department { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;
}


