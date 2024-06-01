using ApplicationCore.Models;
using ApplicationCore.Views;

namespace Web.Models;
public class EventFetchRequest
{
   public EventFetchRequest(Category category)
   {
      Category = category;
   }

   public string Key => Category.Key;
   public Category Category { get; set; }
}
public class EventsIndexModel
{
  
   public EventsIndexModel(EventFetchRequest request, IEnumerable<string> actions)
   {
      Request = request;
      Actions = actions;
   }
   public IEnumerable<string> Actions { get; set; }
   public EventFetchRequest Request { get; set; }

   public IEnumerable<EventViewModel> List { get; set; } = new List<EventViewModel>();

}
public class EventCreateForm
{
   public string Title { get; set; } = String.Empty;
   public string? Content { get; set; }
   public DateTime? StartDate { get; set; }
   public DateTime? EndDate { get; set; }

   public int CategoryId { get; set; }
}
public class EventEditForm : EventCreateForm
{
   
}