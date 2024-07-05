using ApplicationCore.Views;

namespace Web.Models;

public abstract class BaseAttachmentForm
{
   public string PostType { get; set; } = string.Empty;
   public int PostId { get; set; }
   public string? Title { get; set; }
   public string? Description { get; set; }
}


public class AttachmentCreateForm : BaseAttachmentForm
{
   public IFormFile File { get; set; }
}
public class AttachmentEditForm : BaseAttachmentForm
{
   public IFormFile File { get; set; }
}