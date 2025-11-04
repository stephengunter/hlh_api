using ApplicationCore.Views.IT;

namespace Web.Models.IT;


public class FixRecordsIndexModel
{
   public FixRecordsIndexModel(ICollection<int> years, FixRecordsFetchRequest request)
   {
      Years = years;
      Request = request;
      Labels = new FixRecordLabel();
   }

   public ICollection<int> Years { get; set; }
   public FixRecordsFetchRequest Request { get; set; }
   public FixRecordLabel Labels { get; set; }
}

public class FixRecordsFetchRequest
{
   public FixRecordsFetchRequest(int year, int month)
   {
      Year = year;
      Month = month;
   }
   public int Year { get; set; }
   public int Month { get; set; }
}