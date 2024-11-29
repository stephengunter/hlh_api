using ApplicationCore.Consts;
using ApplicationCore.Views.IT;
using Infrastructure.Helpers;

namespace Web.Models.IT;

public class ServerLabels
{
   public string Type => "類型";
   public string Title => "名稱";
   public string Key => "Key";
   public string Provider => "Provider";
   public string Root => "根目錄";
   public string Ps => "備註";
   public string Host => "主機";
}
public class ServerProviders
{
   public ICollection<string> Web => ObjectsHelpers.GetStaticKeys<WebProvider>();
   public ICollection<string> Db => ObjectsHelpers.GetStaticKeys<DbProvider>();
   public ICollection<string> Ftp => ObjectsHelpers.GetStaticKeys<FtpProvider>();
}
public class ServersIndexModel
{
   public ServersIndexModel(ServersFetchRequest request)
   {
      Request = request;
      Providers = new ServerProviders();
   }
   public ServerLabels Labels => new ServerLabels();
   public CredentialInfoLabels CredentialInfoLabels => new CredentialInfoLabels();
   public ServersFetchRequest Request { get; set; }
   public ServerProviders Providers { get; set; }
}

public class ServersFetchRequest
{
   public ServersFetchRequest(string type)
   {
      Type = type;
   }
   public string Type { get; set; }
}
public class ServersAddRequest
{
   public ServersAddRequest(ServerAddForm form, ICollection<HostViewModel> hosts)
   {
      Form = form;
      Hosts = hosts;
   }
   public ServerAddForm Form { get; set; }
   public ICollection<HostViewModel> Hosts { get; set; }
}
public class ServersEditRequest
{
   public ServersEditRequest(ServerEditForm form, ICollection<HostViewModel> hosts)
   {
      Form = form;
      Hosts = hosts;
   }
   public ServerEditForm Form { get; set; }
   public ICollection<HostViewModel> Hosts { get; set; }
}
public abstract class ServerBaseForm
{
   public int HostId { get; set; }
   public string Type { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Provider { get; set; } = string.Empty; 
   public string Root { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
}
public class ServerAddForm : ServerBaseForm
{

}
public class ServerEditForm : ServerBaseForm
{

}