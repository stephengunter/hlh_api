using ApplicationCore.Models.Fetches;
using ApplicationCore.Views;
using ApplicationCore.Views.Criminals;
using ApplicationCore.Views.Fetches;
using System.Drawing;
using System.Numerics;

namespace Web.Models.IT;

public class DatabaseLabels
{
   public string Title => "名稱";
   public string Key => "Key";
   public string Provider => "Provider";
   public string Ps => "備註";
   public string Host => "主機";
}
public class DatabasesIndexModel
{
   public DatabasesIndexModel(DatabasesFetchRequest request, ICollection<string> providers)
   {
      Request = request;
      Providers = providers;
   }
   public DatabaseLabels Labels => new DatabaseLabels();
   public DatabasesFetchRequest Request { get; set; }
   public ICollection<string> Providers { get; set; }
}

public class DatabasesFetchRequest
{
   public DatabasesFetchRequest(int page, int pageSize)
   {
      Page = page;
      PageSize = pageSize;
   }
   
   public int Page { get; set; }
   public int PageSize { get; set; }
}
public class DatabaseAddForm
{
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Provider { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
   public int HostId { get; set; }

}