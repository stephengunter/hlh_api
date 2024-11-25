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
   public string Type => "����";
   public string Title => "���D";
   public string Name => "Name";
   public string Ps => "�Ƶ�";
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
   public DbBackupPlanLabels BackupPlanLabels => new DbBackupPlanLabels();
   public DatabasesFetchRequest Request { get; set; }
   public ICollection<ServerViewModel> Servers { get; set; }
}

public class DatabasesFetchRequest
{
   public DatabasesFetchRequest()
   {
      
   }
   public int? ServerId { get; set; }
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
public class DatabaseEditRequest
{
   public DatabaseEditRequest(DatabaseEditForm form, ICollection<ServerViewModel> servers)
   {
      Form = form;
      Servers = servers;
   }
   public DatabaseEditForm Form { get; set; }
   public ICollection<ServerViewModel> Servers { get; set; }
}