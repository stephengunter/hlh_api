using ApplicationCore.Models.Fetches;
using ApplicationCore.Views;
using ApplicationCore.Views.Criminals;
using ApplicationCore.Views.Fetches;
using System;
using System.Drawing;
using System.Numerics;

namespace Web.Models.IT;

public class HostLabels
{
   public string IP => "IP";
   public string Title => "¦WºÙ";
   public string Key => "Key";
   public string Ps => "³Æµù";

}
public class HostsIndexModel
{
   public HostsIndexModel(HostsFetchRequest request, ICollection<string> providers)
   {
      Request = request;
      Providers = providers;
   }
   public HostLabels Labels => new HostLabels();
   public CredentialInfoLabels CredentialInfoLabels => new CredentialInfoLabels();
   public HostsFetchRequest Request { get; set; }
   public ICollection<string> Providers { get; set; }
}

public class HostsFetchRequest
{
   public HostsFetchRequest(bool active, int page, int pageSize)
   {
      Active = active;
      Page = page;
      PageSize = pageSize;
   }
   public bool Active { get; set; }
   public int Page { get; set; }
   public int PageSize { get; set; }
}

public abstract class BaseHostForm
{
   public string IP { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Ps { get; set; } = string.Empty;
   public bool Active { get; set; }

}
public class HostAddForm : BaseHostForm
{

}
public class HostEditForm : BaseHostForm
{
   public bool CanRemove { get; set; }
}

public abstract class HostBaseRequest
{ 
   
}
public class HostAddRequest : HostBaseRequest
{
   public HostAddRequest(HostAddForm form)
   {
      Form = form;
   }
   public HostAddForm Form { get; set; }
}

public class HostEditRequest : HostBaseRequest
{
   public HostEditRequest(HostEditForm form)
   {
      Form = form;
   }
   public HostEditForm Form { get; set; }
}