using ApplicationCore.Consts;
using ApplicationCore.Views.IT;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace Web.Models;
public class SystemAppFetchRequest
{
   public SystemAppFetchRequest(bool active, int page, int pageSize, string? keyword)
   {
      Active = active;
      Page = page;
      PageSize = pageSize;
      Keyword = keyword;
   }
   public bool Active { get; set; }
   public int Page { get; set; }
   public int PageSize { get; set; }
   public string? Keyword { get; set; }
}

public class SystemAppLabels
{
   public string Title => "�W��";
   public string Type => "����";
   public string Server => "Server";
   public string Importance => "���n��";
   public string ParentId => "���t��";
   public string Key => "Key";
   public string Ps => "�Ƶ�";
}
public class SystemAppsIndexModel
{
   public SystemAppsIndexModel(SystemAppFetchRequest request)
   {
      Request = request;
      Labels = new SystemAppLabels();
      TypeOptions = new List<BaseOption<string>>
      {
         new BaseOption<string>(AppTypes.Web, "Web"),
         new BaseOption<string>(AppTypes.PC, "PC ���ε{��"),
         new BaseOption<string>(AppTypes.Phone, "���App"),
      };
      ImportanceOptions = EnumHelpers.EnumToBaseOptions<Importance>();
   }
   public ICollection<BaseOption<int>> ImportanceOptions { get; set; }
   public ICollection<BaseOption<string>> TypeOptions { get; set; }
   public SystemAppFetchRequest Request { get; set; }
   public SystemAppLabels Labels { get; set; }

}


public abstract class SystemAppBaseForm
{
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Type { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
   public int Importance { get; set; }
   public int? ParentId { get; set; }
   public int ServerId { get; set; }

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