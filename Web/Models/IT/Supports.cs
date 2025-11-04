using ApplicationCore.Views.IT;

namespace Web.Models.IT;

public class SupportRecordsIndexModel
{
   public SupportRecordsIndexModel(ICollection<int> years, SupportRecordsFetchRequest request)
   {
      Years = years;
      Request = request;
      Labels = new SupportLabel();
   }

   public ICollection<int> Years { get; set; }
   public SupportRecordsFetchRequest Request { get; set; }
   public SupportLabel Labels { get; set; }
}

public class SupportRecordsFetchRequest
{
   public SupportRecordsFetchRequest(int year, int month)
   {
      Year = year;
      Month = month;
   }
   public int Year { get; set; }
   public int Month { get; set; }
}