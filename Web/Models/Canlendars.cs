using ApplicationCore.Models;
using ApplicationCore.Views;

namespace Web.Models;
// public class CanlendarFetchRequest
// {
//    public CanlendarRequest(string categoryKeys, int year, int month)
//    {
//       Category = category;
//       Year = year;
//       Month = month;
//    }
//    public int Year { get; set; }
//    public int Month { get; set; }
//    public string Key => Category.Key;
//    public Category Category { get; set; }
// }
// public class CanlendarResponse
// {
//    public CanlendarResponse(CanlendarRequest request)
//    {
//       Request = request;
//    }
//    public CanlendarRequest Request { get; set; }
//    public IEnumerable<EventViewModel> List { get; set; } = new List<EventViewModel>();
// }

public class CanlendarsIndexModel
{

   public IEnumerable<CalendarViewModel> List { get; set; } = new List<CalendarViewModel>();

}