using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views.Judicial;
using Infrastructure.Paging;

namespace Web.Models;


public abstract class BaseItemForm
{
   public string PostType { get; set; } = String.Empty;
   public int PostId { get; set; }

   
   public string Title { get; set; } = String.Empty;
   public bool Done { get; set; }
   public int Order { get; set; }

}
public class ItemCreateForm : BaseItemForm
{
   public ICollection<int>? AttachmentIds { get; set; }
}
public class ItemEditForm : BaseItemForm
{
   public ICollection<int>? AttachmentIds { get; set; }
}