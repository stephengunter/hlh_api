using Infrastructure.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Views.Fetches;
public class FetchesRecordView : EntityBaseView
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
   public string QueryTime { get; set; } = string.Empty;
   public string QueryKey { get; set; } = string.Empty;
   public string IP { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;

   //public string PsText
   //{
   //   get
   //   {
   //      string text = "";
   //      switch (Ps)
   //      {
   //         case "*":
   //            text = "臨時查詢";
   //            break;
   //         case "%":
   //            text = "未登資料查詢";
   //            break;
   //         case "$":
   //            text = "未檢核案號";
   //            break;
   //         case "&":
   //            text = "檢核連線異常";
   //            break;
   //         case "":
   //            text = "檢核正常";
   //            break;
   //         default:
   //            break;
   //      }

   //      return $"{Ps}({text})";
   //   }
   //}

   public object this[string propertyName]
   {
      get
      {
         return propertyName switch
         {
            "SystemId" => SystemId,
            "Name" => Name,
            "Identifier" => Identifier,
            "CourtType" => CourtType,
            "CaseNumber" => CaseNumber,
            "Year" => Year,
            "Month" => Month,
            "Day" => Day,
            "Time" => Time,
            "QueryTime" => QueryTime,
            "QueryKey" => QueryKey,
            "IP" => IP,
            "Ps" => Ps,
            _ => throw new ArgumentException("Invalid property name", nameof(propertyName))
         };
      }
   }
}
public class FetchesSystemView : EntityBaseView
{
   public string Department { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;
}

