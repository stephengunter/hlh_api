using ApplicationCore.Views;

namespace Web.Models;

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