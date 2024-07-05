using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views.Judicial;
using Infrastructure.Paging;

namespace Web.Models;
public class TasksFetchRequest : PageableRequest
{
   public TasksFetchRequest(int page, int pageSize) : base(page, pageSize)
   {
      
   }

}
public class TasksIndexModel
{
  
   public TasksIndexModel(TasksFetchRequest request, IEnumerable<string> actions)
   {
      Request = request;
      Actions = actions;
   }
   public IEnumerable<string> Actions { get; set; }
   public TasksFetchRequest Request { get; set; }

   public PagedList<Tasks, TaskViewModel>? PagedList { get; set; }

}

public abstract class BaseTaskForm
{
   public string Title { get; set; } = String.Empty;
   public string? Content { get; set; }
   public DateTime? DeadLine { get; set; }
   public int? ParentId { get; set; }

}
public class TaskCreateForm : BaseTaskForm
{
   public ICollection<ReferenceCreateForm> References { get; set; } = new List<ReferenceCreateForm>();
}
public class TaskEditForm : BaseTaskForm
{
   public ICollection<ReferenceEditForm> References { get; set; } = new List<ReferenceEditForm>();
}