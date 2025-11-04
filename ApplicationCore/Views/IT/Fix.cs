using Infrastructure.Helpers;
using Infrastructure.Entities;
using Infrastructure.Views;

namespace ApplicationCore.Views.IT;
public class FixViewModel
{
   public string Number { get; set; } = string.Empty;
   public string Date { get; set; } = string.Empty;
   public string Department { get; set; } = string.Empty;
   public string User { get; set; } = string.Empty;
   public string Kind { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;
   public string Content { get; set; } = string.Empty;
   public string Result { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
   public int Count { get; set; }
}


public class FixRecordLabel
{
   public string Date => "日期";
   public string Number => "單號";
   public string User => "申請人";
   public string Content => "設備名稱";
   public string Count => "數量";
   public string Result => "說明";
   public string Ps => "備註";
}
