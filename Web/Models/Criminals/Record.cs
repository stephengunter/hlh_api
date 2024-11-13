using ApplicationCore.Views;
using ApplicationCore.Views.Criminals;

namespace Web.Models.Criminals;

public class CriminalFetchRecordLabel
{
   public string Title = "¦WºÙ";
   public string Key = "Key";
}

public class CriminalFetchRecordsIndexModel
{
   public CriminalFetchRecordsIndexModel(ICollection<int> years, CriminalFetchRecordsFetchRequest request)
   {
      Years = years;
      Request = request;
   }
   public ICollection<int> Years { get; set; }
   public CriminalFetchRecordsFetchRequest Request { get; set; }
}

public class CriminalFetchRecordsFetchRequest
{
   public CriminalFetchRecordsFetchRequest(int year, int month, int page, int pageSize)
   {
      Year = year;
      Month = month;
      Page = page;
      PageSize = pageSize;
   }
   public int Year { get; set; }
   public int Month { get; set; }
   public int Page { get; set; }
   public int PageSize { get; set; }
}
public class CriminalFetchRecordsUploadRequest
{
   public IFormFile? File { get; set; }
}

public class CriminalFetchRecordsAddRequest
{ 
   public int Year { get; set; }
   public int Month { get; set; }

   public ICollection<CriminalFetchRecordView> Records { get; set; } = new List<CriminalFetchRecordView>();
}