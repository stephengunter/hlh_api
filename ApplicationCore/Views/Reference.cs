using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class Reference
{
   public int? AttachmentId { get; set; }
   public string Title { get; set; } = String.Empty;
   public string Url { get; set; } = String.Empty;
}

