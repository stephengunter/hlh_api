using ApplicationCore.Models.Fetches;
using ApplicationCore.Views;
using ApplicationCore.Views.Criminals;
using ApplicationCore.Views.Fetches;
using System.Drawing;
using System.Numerics;

namespace Web.Models.Fetches;
public class FetchRecordsIndexModel
{
   public FetchRecordsIndexModel(ICollection<FetchSystemDepartmentView> systemDepartments, ICollection<FetchesSystem> systems, ICollection<int> years, FetchRecordsFetchRequest request)
   {
      SystemDepartments = systemDepartments;
      Systems = systems;
      Years = years;
      Request = request;
   }
   public ICollection<FetchSystemDepartmentView> SystemDepartments { get; set; }
   public ICollection<FetchesSystem> Systems { get; set; }
   public ICollection<int> Years { get; set; }
   public FetchRecordsFetchRequest Request { get; set; }
}
public class FetchRecordsSummaryIndex
{
   public ICollection<FetchRecordsSummary> Summaries { get; set; } = new List<FetchRecordsSummary>();
   public int Total => Summaries.Sum(item => item.Count);

}

public class FetchRecordsFetchRequest
{
   public FetchRecordsFetchRequest(int system, int year, int month)
   {
      System = system;
      Year = year;
      Month = month;
   }
   public int System { get; set; }
   public int Year { get; set; }
   public int Month { get; set; }
}
public class FetchRecordsDownloadRequest
{
   public FetchRecordsDownloadRequest(int year, int month)
   {
      Year = year;
      Month = month;
   }
   public int Year { get; set; }
   public int Month { get; set; }
}
public class FetchRecordsUploadRequest
{
   public IFormFile? File { get; set; }
}

public class FetchRecordsAddRequest
{
   public int System { get; set; }
   public int Year { get; set; }
   public int Month { get; set; }
   public ICollection<FetchesRecordView> Records { get; set; } = new List<FetchesRecordView>();

}

public class FetchSystemDepartmentView
{
   public string Key { get; set; }
   public string Title { get; set; }

   public FetchSystemDepartmentView(string key, string title)
   {
      Key = key;
      Title = title;
   }
}