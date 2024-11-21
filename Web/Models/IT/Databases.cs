using ApplicationCore.Models.Fetches;
using ApplicationCore.Views;
using ApplicationCore.Views.Criminals;
using ApplicationCore.Views.Fetches;
using ApplicationCore.Views.IT;
using Infrastructure.Views;
using System.Drawing;
using System.Numerics;

namespace Web.Models.IT;

public class DatabaseLabels
{
   public string Type => "類型";
   public string Title => "標題";
   public string Name => "Name";
   public string Ps => "備註";
   public string Server => "Server";
}
public class DatabasesIndexModel
{
   public DatabasesIndexModel(DatabasesFetchRequest request, ICollection<ServerViewModel> servers)
   {
      Request = request;
      Servers = servers;
   }
   public DatabaseLabels Labels => new DatabaseLabels();
   public DatabasesFetchRequest Request { get; set; }
   public ICollection<ServerViewModel> Servers { get; set; }
}

public class DatabasesFetchRequest
{
   public DatabasesFetchRequest()
   {
      
   }
   public int ServerId { get; set; }
}
public abstract class DatabaseBaseForm
{
   public string Title { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;
   public string Ps { get; set; } = string.Empty;
   public int ServerId { get; set; }

}
public class DatabaseAddForm : DatabaseBaseForm
{

}
public class DatabaseEditForm : DatabaseBaseForm
{

}