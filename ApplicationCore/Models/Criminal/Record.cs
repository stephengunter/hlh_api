using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.Criminals;

[Table("Criminal.FetchRecords")]
public class CriminalFetchRecord : EntityBase
{
   public string UserCode { get; set; } = string.Empty;
   public string UserName { get; set; } = string.Empty;
   public string QueryKind { get; set; } = string.Empty;
   public string Query { get; set; } = string.Empty;
   public int Year { get; set; }
   public int Month { get; set; }
   public int Day { get; set; }
   public string Time { get; set; } = string.Empty;
   public string DataKind { get; set; } = string.Empty;
   public string IP { get; set; } = string.Empty;
   public string Result { get; set; } = string.Empty;
}


