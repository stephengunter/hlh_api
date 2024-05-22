using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class TagViewModel : EntityBaseView, IBaseRecordView, IBaseCategoryView<TagViewModel>
{
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;

   public TagViewModel? Parent { get; set; }
   public int? ParentId { get; set; }
   public bool IsRootItem { get; set; }
   public ICollection<TagViewModel>? SubItems { get; set; }
   public ICollection<int>? SubIds { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }

   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateString();
   public string LastUpdatedText => LastUpdated.ToDateString();

}

