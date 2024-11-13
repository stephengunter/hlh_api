using Infrastructure.Entities;

namespace ApplicationCore.Views.Criminals;
public class CriminalFetchRecordView : EntityBaseView
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

