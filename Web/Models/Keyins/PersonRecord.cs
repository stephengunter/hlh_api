using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views;
using ApplicationCore.Views.Keyin;
using Infrastructure.Paging;

namespace Web.Models.Keyin;

public class PersonRecordLabel
{
   public string Title = "¦WºÙ";
   public string Key = "Key";
}

public class PersonRecordsIndexModel
{
   public PersonRecordsIndexModel(ICollection<int> years, PersonRecordsFetchRequest request, ICollection<KeyinPersonView> persons)
   {
      Years = years;
      Request = request;
      Persons = persons;
      Labels = new PersonRecordLabel();
   }

   public ICollection<KeyinPersonView> Persons { get; set; }
   public ICollection<int> Years { get; set; }
   public PersonRecordsFetchRequest Request { get; set; }
   public PersonRecordLabel Labels { get; set; }
}

public class PersonRecordsFetchRequest
{
   public PersonRecordsFetchRequest(int year, int month)
   {
      Year = year;
      Month = month;
   }
   public int Year { get; set; }
   public int Month { get; set; }
}
public class PersonRecordsUploadRequest
{
   public int Year { get; set; }
   public int Month { get; set; }
   public IFormFile? File { get; set; }
}

public class PersonRecordsAddRequest
{
   public int Year { get; set; }
   public int Month { get; set; }

   public ICollection<PersonRecordView> Records { get; set; } = new List<PersonRecordView>();
}