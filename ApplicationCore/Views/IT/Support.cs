namespace ApplicationCore.Views.IT;
public class SupportViewModel
{
   public string Date { get; set; } = string.Empty;
   public string Department { get; set; } = string.Empty;
   public string User { get; set; } = string.Empty;
   public string Kind { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;
   public string Content { get; set; } = string.Empty;
   public string Result { get; set; } = string.Empty;
   public string Person { get; set; } = string.Empty;
   public int PersonCount { get; set; }
}

public class ITSupportGroup
{
   public string Title { get; set; } = string.Empty;
   public List<SupportViewModel> Records { get; set; } = new List<SupportViewModel>();

   public int TotalCount => Records.Sum(r => r.PersonCount);
}

public class SupportLabel
{
   public string Date => "日期";
   public string User => "使用者";
   public string Name => "系統名稱";
   public string Kind => "問題類別";
   public string Content => "問題簡述";
   public string Result => "處理狀況";
   public string Person => "處理人員";
   public string PersonCount => "處理人次";
}
