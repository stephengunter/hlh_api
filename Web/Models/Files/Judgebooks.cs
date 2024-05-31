using ApplicationCore.Models.Files;
using ApplicationCore.Views.Files;
using Azure.Core;
using Infrastructure.Paging;

namespace Web.Models.Files;

#region Request
public class JudgebookFilesAdminRequest : BaseJudgebookRequest
{
   public JudgebookFilesAdminRequest(JudgebookFile model, int reviewed, int page = 1, int pageSize = 10)
   {
      if (reviewed == 0 || reviewed == 1) Reviewed = reviewed;
      else Reviewed = -1;

      Page = page < 1 ? 1 : page;
      PageSize = pageSize < 1 ? 1 : pageSize;

      TypeId = model.TypeId;
      CourtType = model.CourtType;
      OriginType = model.OriginType;
      FileNumber = model.FileNumber;

      Year = model.Year;
      Category = model.Category;
      Num = model.Num;

   }
   public int Reviewed { get; set; }
  
   public int Page { get; set; }
   public int PageSize { get; set; }

}

public class JudgebookUpdateRequest : BaseJudgebookRequest
{
   public string FileName { get; set; } = String.Empty;
   public bool Reviewed { get; set; }
}
public class JudgebookUploadRequest : BaseJudgebookRequest
{
   public int Id { get; set; }
}

public class JudgebookReviewRequest
{
   public int Id { get; set; }

   public string FileNumber { get; set; } = String.Empty;
   public string? Ps { get; set; }
}



#endregion


public class JudgebookFilesAdminModel
{
   public JudgebookFilesAdminModel(JudgebookFilesAdminRequest request, IEnumerable<string> actions)
   {
      Request = request;
      Actions = actions;
   }
   public bool AllowEmptyJudgeDate{ get; set; }
   public bool AllowEmptyFileNumber { get; set; }
   public IEnumerable<string> Actions { get; set; }
   public JudgebookFilesAdminRequest Request { get; set; }

   public PagedList<JudgebookFile, JudgebookFileViewModel>? PagedList { get; set; }

}

public class JudgebookFileEditModel
{
   public JudgebookFileEditModel(JudgebookFileViewModel model, IEnumerable<string> actions)
   {
      Model = model;
      Actions = actions;
   }
   public JudgebookFileViewModel Model { get; set; }
   public IEnumerable<string> Actions { get; set; }

}

public class JudgebookFileUploadResponse
{
   public int id { get; set; }
   public JudgebookFileViewModel? Model { get; set; }
   public Dictionary<string, string>? Errors { get; set; }
}

