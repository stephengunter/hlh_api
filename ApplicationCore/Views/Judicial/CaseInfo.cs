using Infrastructure.Entities;
using Infrastructure.Helpers;
using ApplicationCore.Consts;

namespace ApplicationCore.Views.Judicial;

public class CaseInfoViewModel
{
   public string CourtType { get; set; } = JudgeCourtTypes.H;
   public string Year { get; set; } = String.Empty;
   public string Category { get; set; } = String.Empty;
   public string Num { get; set; } = String.Empty;
}


