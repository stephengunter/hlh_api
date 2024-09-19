using ApplicationCore.Models;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class BranchViewModel : EntityBaseView
{
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }
}

