using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views.Judicial;

namespace Web.Models;
public class TasksFetchRequest
{
   public TasksFetchRequest()
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

   public IEnumerable<TaskViewModel> List { get; set; } = new List<TaskViewModel>();

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
}
public class TaskEditForm : BaseTaskForm
{
   
}