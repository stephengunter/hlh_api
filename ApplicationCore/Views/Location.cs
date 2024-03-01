using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class LocationViewModel : EntityBaseView, IBaseCategoryView<LocationViewModel>
{
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public LocationViewModel? Parent { get; set; }
   public int? ParentId { get; set; }
   public bool IsRootItem { get; set; }
   public ICollection<LocationViewModel>? SubItems { get; set; }
   public ICollection<int>? SubIds { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }

   public bool Active { get; set; }
}

