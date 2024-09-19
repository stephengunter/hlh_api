using ApplicationCore.Models;
using ApplicationCore.Models.IT;
using ApplicationCore.Views;
using ApplicationCore.Views.IT;
using Infrastructure.Paging;

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

public class SystemAppLabel
{
   public string Title = "¦WºÙ";
   public string Key = "Key";
}
public class SystemAppsIndexModel
{
   public SystemAppsIndexModel(SystemAppFetchRequest request)
   {
      Request = request;
      Labels = new SystemAppLabel();
   }

   public SystemAppFetchRequest Request { get; set; }
   public SystemAppLabel Labels { get; set; }

}