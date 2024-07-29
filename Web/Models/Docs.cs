using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views.Judicial;
using Infrastructure.Paging;

namespace Web.Models;

public class DocsFetchRequest : PageableRequest
{
   public DocsFetchRequest(int page, int pageSize) : base(page, pageSize)
   {
      
   }

}
public class DocsIndexModel
{

   public ICollection<TelName> Departments { get; set; } = new List<TelName>();

}