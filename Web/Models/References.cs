using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views.Judicial;
using Infrastructure.Paging;

namespace Web.Models;


public abstract class BaseReferenceForm
{
   public string PostType { get; set; } = String.Empty;
   public int PostId { get; set; }

   public int? AttachmentId { get; set; }
   public string Title { get; set; } = String.Empty;
   public string Url { get; set; } = String.Empty;
   public int Order { get; set; }

}
public class ReferenceCreateForm : BaseReferenceForm
{
   
}
public class ReferenceEditForm : BaseReferenceForm
{

}