using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views.Judicial;
using ApplicationCore.Views.Keyin;
using Infrastructure.Paging;

namespace Web.Models.Keyin;

public class BranchRecordLabel
{
   public string Title = "¦WºÙ";
   public string Key = "Key";
}

public class BranchRecordsIndexModel
{
   public BranchRecordsIndexModel(ICollection<int> years, BranchRecordsFetchRequest request, ICollection<BranchViewModel> branches)
   {
      Years = years;
      Request = request;
      Branches = branches;
      Labels = new BranchRecordLabel();
   }

   public ICollection<BranchViewModel> Branches { get; set; }
   public ICollection<int> Years { get; set; }
   public BranchRecordsFetchRequest Request { get; set; }
   public BranchRecordLabel Labels { get; set; }
}

public class BranchRecordsFetchRequest
{
   public BranchRecordsFetchRequest(int year, int month)
   {
      Year = year;
      Month = month;
   }
   public int Year { get; set; }
   public int Month { get; set; }
}
public class BranchRecordsUploadRequest
{
   public IFormFile? File { get; set; }
}

public class BranchRecordsAddRequest
{ 
   public int Year { get; set; }
   public int Month { get; set; }

   public ICollection<BranchRecordView> Records { get; set; } = new List<BranchRecordView>();
}