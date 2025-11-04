using Infrastructure.Views;

namespace ApplicationCore.Views.Keyin;
public class KeyinPersonView : EntityBaseView
{
   public string Account { get; set; } = string.Empty;
   public string Unit { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;

   public bool AllPass { get; set; }

   public string LeaveAtText { get; set; } = string.Empty;
   public bool Active { get; set; }
   public int HighRun { get; set; }

   public string AllPassText => AllPass ? "免測" : "";
}

public class KeyinPersonLabels
{
   public string Account => "帳號";
   public string Unit => "股別";
   public string Name => "姓名";
   public string HighRun => "最佳成績";
   public string LeaveAtText => "離職日期";
}


