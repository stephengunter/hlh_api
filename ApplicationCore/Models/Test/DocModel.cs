using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;

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
   public int Flag { get; set; }
}


