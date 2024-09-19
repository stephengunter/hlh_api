using Infrastructure.Entities;

namespace ApplicationCore.Views.Keyin;
public class BranchRecordView : EntityBaseView
{
   public int Rank { get; set; }
   public int Year { get; set; }
   public int Month { get; set; }
   public int BranchId { get; set; }
   public int Score { get; set; }

   public string AbsentRate { get; set; } = string.Empty;
   public string Diff { get; set; } = string.Empty;
   public BranchViewModel? Branch { get; set; }
}


public class BranchRecordLabels
{
   public static string Rank = "名次";
   public static string Year = "年度";
   public static string Month = "月份";
   public static string BranchId = "法院";
   public static string Score = "成績(字/分)";

   public static string AbsentRate = "缺考率";
   public static string Diff = "進步率(%)";
}

public class PersonRecordView : EntityBaseView
{
   public int Year { get; set; }
   public int Month { get; set; }
   public int PersonId { get; set; }
   public int Score { get; set; }

   public string Unit { get; set; } = string.Empty;
   public string CorrectRate { get; set; } = string.Empty;
   public string Diff { get; set; } = string.Empty;
   public KeyinPersonView? Person { get; set; }
}

public class PersonRecordLabels
{
   
   public static string Year = "年度";
   public static string Month = "月份";
   public static string Unit = "股別";
   public static string PersonId = "姓名";
   public static string Score = "成績(字/分)";

   public static string CorrectRate = "正確率(%)";
   public static string Diff = "進步率(%)";
}

