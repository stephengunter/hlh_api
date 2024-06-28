using ApplicationCore.Helpers;
using Infrastructure.Helpers;

namespace Web.Models;

public abstract class PageableRequest
{
	public PageableRequest(int page, int pageSize)
   {
      Page = page;
      PageSize = pageSize;
   }

   public int Page { get; set; }
	public int PageSize { get; set; }
}