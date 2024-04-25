using ApplicationCore.Models.Files;
using ApplicationCore.Views.Files;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace Web.Models.Files;
public class JudgebookFilesAdminRequest
{
   public JudgebookFilesAdminRequest(string year = "", string category = "", string num = "", int page = 1, int pageSize = 10)
   {
      var model = new JudgebookFile(year, category, num);
      Year = model.Year;
      Category = model.Category;
      Num = model.Num;
      Page = page < 1 ? 1 : page;
      PageSize = pageSize < 1 ? 1 : pageSize;
   }
   public string Year { get; set; }
   public string Category { get; set; }
   public string Num { get; set; }
   public int Page { get; set; }
   public int PageSize { get; set; }

}


public class JudgebookFilesAdminModel
{
   public JudgebookFilesAdminModel(JudgebookFilesAdminRequest request)
   {
      Request = request;
   }

   public JudgebookFilesAdminRequest Request { get; set; }

   public PagedList<JudgebookFile, JudgebookFileViewModel>? PagedList { get; set; }

}
public class JudgebookUploadRequest
{
   public int Id { get; set; }
   public string Year { get; set; } = String.Empty;
   public string Category { get; set; } = String.Empty;
   public string Num { get; set; } = String.Empty;
   public string? Ps { get; set; }
   public string? Type { get; set; }
   public IFormFile File { get; set; }
}
public class JudgebookFileUploadResponse
{
   public int id { get; set; }
   public JudgebookFileViewModel? Model { get; set; }
   public Dictionary<string, string>? Errors { get; set; }
}
