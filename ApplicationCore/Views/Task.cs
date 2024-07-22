using ApplicationCore.Views;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Models;

public class TaskViewModel : EntityBaseView, IBaseCategoryView<TaskViewModel>, IBaseRecordView
{
   public string UserId { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;

   public string Content { get; set; } = String.Empty;

   public DateTime? DeadLine { get; set; }

   public bool Done { get; set; }

   public TaskViewModel? Parent { get; set; }

   public int? ParentId { get; set; }

   public bool IsRootItem { get; set; }

   public ICollection<TaskViewModel>? SubItems { get; set; }
  
   public ICollection<int>? SubIds { get; set; }

   public string DeadLineText => DeadLine.ToDateString();

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();
   

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }

   public ICollection<ReferenceViewModel> References { get; set; } = new List<ReferenceViewModel>();
}