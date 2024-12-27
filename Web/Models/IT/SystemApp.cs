using ApplicationCore.Consts;
using ApplicationCore.Views.IT;
using Infrastructure.Helpers;
using Infrastructure.Views;
using Web.Models.IT;

namespace Web.Models;
public class SystemAppFetchRequest
{
   public SystemAppFetchRequest(bool active, int centralized, int page, int pageSize, string? keyword)
   {
      Active = active;
      Centralized = centralized;
      Page = page;
      PageSize = pageSize;
      Keyword = keyword;
   }
   public bool Active { get; set; }
   public int Centralized { get; set; }
   public int Page { get; set; }
   public int PageSize { get; set; }
   public string? Keyword { get; set; }
}

public class SystemAppLabels
{
   public string Title => "名稱";
   public string Type => "類型";
   public string Centralized => "集中化";
   public string Server => "Server";
   public string Importance => "重要性";
   public string ParentId => "父系統";
   public string Key => "Key";
   public string Ps => "備註";
   public string Url => "網址";
}
public class SystemAppsIndexModel
{
   public SystemAppsIndexModel(SystemAppFetchRequest request)
   {
      Request = request;
      CentralizedOptions = new List<BaseOption<int>>
      {
         new BaseOption<int>(-1, "全部"),
         new BaseOption<int>(0, "本院維運"),
         new BaseOption<int>(1, "集中化"),
      };
      TypeOptions = new List<BaseOption<string>>
      {
         new BaseOption<string>(AppTypes.Web, "Web"),
         new BaseOption<string>(AppTypes.PC, "PC 應用程式"),
         new BaseOption<string>(AppTypes.Phone, "手機App"),
      };
      ImportanceOptions = EnumHelpers.EnumToBaseOptions<Importance>();
   }
   public ICollection<BaseOption<int>> CentralizedOptions { get; set; }
   public ICollection<BaseOption<int>> ImportanceOptions { get; set; }
   public ICollection<BaseOption<string>> TypeOptions { get; set; }
   public SystemAppFetchRequest Request { get; set; }
   public SystemAppLabels Labels => new SystemAppLabels();
   public CredentialInfoLabels CredentialInfoLabels => new CredentialInfoLabels();

}


public abstract class SystemAppBaseForm
{
   public bool Centralized { get; set; }
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Type { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
   public int Importance { get; set; }
   public int? ParentId { get; set; }
   public int? ServerId { get; set; }
   public string Url { get; set; } = string.Empty;

}
public class SystemAppAddForm : SystemAppBaseForm
{

}
public class SystemAppEditForm : SystemAppBaseForm
{

}
public class SystemAppAddRequest
{
   public SystemAppAddRequest(SystemAppAddForm form, ICollection<ServerViewModel> servers)
   {
      Form = form;
      Servers = servers;
   }
   public SystemAppAddForm Form { get; set; }
   public ICollection<ServerViewModel> Servers { get; set; }
}
public class SystemAppEditRequest
{
   public SystemAppEditRequest(SystemAppEditForm form, ICollection<ServerViewModel> servers)
   {
      Form = form;
      Servers = servers;
   }
   public SystemAppEditForm Form { get; set; }
   public ICollection<ServerViewModel> Servers { get; set; }
}