
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers.Files;
using AutoMapper;
using Web.Models.Files;
using Microsoft.Extensions.Options;
using ApplicationCore.Exceptions;
using ApplicationCore.Settings.Files;
using ApplicationCore.Services.Files;
using ApplicationCore.Views.Files;
using ApplicationCore.Models.Files;
using Ardalis.Specification;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Consts;
using QuestPDF.Fluent;
using Infrastructure.Views;
using ApplicationCore.Settings;
using ApplicationCore.Consts;
using ApplicationCore.Models;
using System.Collections.Generic;
using ApplicationCore.Services;

namespace Web.Controllers.Api.Files
{
   [Route("api/files/[controller]")]
   [Authorize(Policy = "JudgebookFiles")]
   public class JudgebooksController : BaseApiController, IDisposable
   {
      private readonly CompanySettings _companySettings;
      private readonly JudgebookFileSettings _judgebookSettings;
      private readonly IJudgebookFilesService _judgebooksService;
      private readonly IFileStoragesService _fileStoragesService;
      private readonly IUsersService _usersService;
      private readonly IMapper _mapper;
      private const string REMOVED = "removed";
      public JudgebooksController(IOptions<CompanySettings> companySettings, IOptions<JudgebookFileSettings> judgebookSettings, 
         IJudgebookFilesService judgebooksService, IUsersService usersService,
         IMapper mapper)
      {
         _companySettings = companySettings.Value;
         _judgebookSettings = judgebookSettings.Value;
         _judgebooksService = judgebooksService;
         _usersService = usersService;
         _mapper = mapper;

         if (String.IsNullOrEmpty(_judgebookSettings.Host))
         {
            _fileStoragesService = new LocalStoragesService(_judgebookSettings.Directory);
         }
         else
         {
            _fileStoragesService = new FtpStoragesService(_judgebookSettings.Host, _judgebookSettings.UserName,
            _judgebookSettings.Password, _judgebookSettings.Directory);
         }
         
      }

      string ReportTitle => $"{_companySettings.Title}{_judgebookSettings.Title}";

      [HttpGet]
      public async Task<ActionResult<JudgebookFilesAdminModel>> Index(int reviewed = -1, int typeId = 0, string courtType = "",
          string fileNumber = "",  string year = "", string category = "", string num = "", int page = 1, int pageSize = 50)
      {
         JudgebookType? type = null;
         if (typeId > 0)
         {
            type = await _judgebooksService.GetTypeByIdAsync(typeId);
            if (type == null)
            {
               ModelState.AddModelError("typeId", $"Type 不存在 Id: ${typeId}");
               return BadRequest(ModelState);
            }
         }
         int judgeDate = 0;
         var request = new JudgebookFilesAdminRequest(new JudgebookFile(type, judgeDate, fileNumber, courtType, year, category, num), reviewed, page, pageSize);
         
         var actions = new List<string>();
         if (CanReview()) actions.Add(ActionsTypes.Review);
         var model = new JudgebookFilesAdminModel(request, actions);
         model.AllowEmptyFileNumber = _judgebookSettings.AllowEmptyFileNumber;
         model.AllowEmptyJudgeDate = _judgebookSettings.AllowEmptyJudgeDate;


         string include = "type";
         var judgebooks = type == null ? 
            await _judgebooksService.FetchAllAsync(include) 
            : await _judgebooksService.FetchAsync(type, include);


         if (request.Reviewed == 0) judgebooks = judgebooks.Where(x => x.Reviewed == false);
         else if (request.Reviewed == 1) judgebooks = judgebooks.Where(x => x.Reviewed == true);

         if (!String.IsNullOrEmpty(request.CourtType)) judgebooks = judgebooks.Where(x => x.CourtType == request.CourtType);
         if (!String.IsNullOrEmpty(request.FileNumber)) judgebooks = judgebooks.Where(x => x.FileNumber == request.FileNumber);
         
         if (!String.IsNullOrEmpty(request.Year)) judgebooks = judgebooks.Where(x => x.Year == request.Year);
         if (!String.IsNullOrEmpty(request.Category)) judgebooks = judgebooks.Where(x => x.Category == request.Category);
         if (!String.IsNullOrEmpty(request.Num)) judgebooks = judgebooks.Where(x => x.Num == request.Num);

         var pagedList = judgebooks.GetOrdered().GetPagedList(_mapper, page, pageSize);
         foreach (var item in pagedList.ViewList)
         {
            item.CanEdit = CanEdit(item);
         }

         model.PagedList = pagedList;

         return model;
      }

      [HttpGet("types")]
      public async Task<ActionResult<IEnumerable<JudgebookTypeViewModel>>> Types()
      {
         var types = await _judgebooksService.FetchTypesAsync();
         return types.MapViewModelList(_mapper);
      }


      [HttpGet("edit/{id}")]
      public async Task<ActionResult<JudgebookFileEditModel>> Edit(int id)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();

         if (!CanEdit(entity)) return Forbid();

         var actions = new List<string> { ActionsTypes.Update };
         if (CanReview()) actions.Add(ActionsTypes.Review);

         return new JudgebookFileEditModel(entity.MapViewModel(_mapper), actions);
      }

      bool CanEdit(IJudgebookFile entity)
      {
         if (CanReview()) return true;

         if (entity.Reviewed) return false;
         return entity.CreatedBy == User.Id();
      }
      bool CanDownload(IJudgebookFile entity) => CanEdit(entity);
      bool CanReview() => User.IsFileManager() || User.IsDev();

      [HttpPut("{id}")]
      public async Task<ActionResult> Update(int id, [FromForm] JudgebookUpdateRequest model)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();

         if (!CanEdit(entity)) return Forbid();


         var type = await _judgebooksService.GetTypeByIdAsync(model.TypeId);
         if (type == null) ModelState.AddModelError("type", "錯誤的typeId");

         var cloneEntity = entity.CloneEntity();

         model.SetValuesTo(entity);
         entity.SetUpdated(User.Id());

         if (entity.Reviewed && !CanReview()) return Forbid();

         var errors = await ValidateModelAsync(entity);
         if (model.HasFile)
         { 
            var fileErrors = ValidateFile(model.File!);
            errors = errors.CombineDictionaries(fileErrors);
         }

         AddErrors(errors);
         if (!ModelState.IsValid) return BadRequest(ModelState);


         if (model.File == null)
         {
            bool sameCase = entity.IsSameCase(cloneEntity);
            if (!sameCase)
            {
               string destPath = MoveFile(entity, removed: false);

               entity.FileName = Path.GetFileName(destPath);
               entity.DirectoryPath = Path.GetDirectoryName(destPath)!;
            }
         }
         else
         {
            MoveFile(cloneEntity, removed: true);

            try
            {
               var file = model.File;
               string filePath = SaveFile(file!, entity);
               entity.FileName = Path.GetFileName(filePath);
               entity.Ext = Path.GetExtension(file!.FileName);
               entity.FileSize = file!.Length;

               entity.Host = _judgebookSettings.Host;
               entity.DirectoryPath = Path.GetDirectoryName(filePath)!;
               
            }
            catch (Exception ex)
            {
               ModelState.AddModelError("file", "檔案上傳失敗");
               return BadRequest(ModelState);
            }
         }

         string ip = RemoteIpAddress;
         await _judgebooksService.UpdateAsync(entity, ip);

         return Ok(entity.MapViewModel(_mapper));
      }

      string SaveFile(IFormFile file, JudgebookFile entry)
      {
         string folderPath = entry.CourtType;
         string ext = Path.GetExtension(file.FileName);
         string fileName = entry.CreateFileName() + ext;   //$"{entry.Year}_{entry.Category}_{entry.Num}";

         return _fileStoragesService.Create(file, folderPath, fileName);
      }

      async Task<JudgebookFileUploadResponse> AddOneAsync(JudgebookUploadRequest request, string ip)
      {
         var result = new JudgebookFileUploadResponse() { id = request.Id };
         var type = await _judgebooksService.GetTypeByIdAsync(request.TypeId);
         var entry = request.CreateEntity(type!);
         var file = request.File;

         var modelErrors = await ValidateModelAsync(entry);
         var fileErrors = ValidateFile(file!);
         var errors = modelErrors.CombineDictionaries(fileErrors);
         
         if (errors.Count > 0)
         {
            result.Errors = errors;
            return result;
         }

         try
         {
            string filePath = SaveFile(file!, entry);
            entry.FileName = Path.GetFileName(filePath);
            entry.Ext = Path.GetExtension(file!.FileName);

            entry.Host = _judgebookSettings.Host;
            entry.DirectoryPath = Path.GetDirectoryName(filePath)!;

            entry.FileSize = file!.Length;
            entry.SetCreated(User.Id());

            entry = await _judgebooksService.CreateAsync(entry, ip);
            if (entry == null)
            {
               errors.Add("create", "create failed");
               result.Errors = errors;
            }
            else
            {
               result.Model = entry.MapViewModel(_mapper);
            }
            return result;
         }
         catch (Exception ex)
         {
            errors.Add("file", $"檔案上傳失敗.");   //{ex.Message}
            result.Errors = errors;
            return result;
         }


      }


      [HttpPost("upload")]
      public async Task<ActionResult> Upload([FromForm] List<JudgebookUploadRequest> models)
      {
         string ip = RemoteIpAddress;
         var resultList = new List<JudgebookFileUploadResponse>();
         for (int i = 0; i < models.Count; i++)
         {
            var result = await AddOneAsync(models[i], ip);
            resultList.Add(result);
         }

         return Ok(resultList);
      }

      [HttpGet("download/{id}")]
      public async Task<ActionResult> Download(int id)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();
         
         if (!CanDownload(entity)) return Forbid();

         byte[] bytes;
         try
         {
            bytes = _fileStoragesService.GetBytes(entity.DirectoryPath, entity.FileName);
         }
         catch (Exception ex)
         {
            if (ex is FileNotExistException)
            {
               throw new FileNotExistException(entity, (ex as FileNotExistException)!.Path);
            }
            throw;
         }

         if(entity.Reviewed) await _judgebooksService.AddDownloadRecordAsync(entity, User.Id(), RemoteIpAddress);

         var model = entity.MapViewModel(_mapper, bytes);
         return Ok(model);
      }
      [HttpGet("reports")]
      public async Task<ActionResult<ICollection<JudgebookFileReportItem>>> Reports(string createdAt = "", string judgeDate = "")
      {
         if (!CanReview()) return Forbid();

         string include = "type";
         var judgebooks = await _judgebooksService.FetchAllAsync(include);
         
         judgebooks = judgebooks.Where(x => x.Reviewed == true);

         if (!string.IsNullOrEmpty(createdAt))
         {
            string[] parts = createdAt.Split(new string[] { "~" }, StringSplitOptions.None);
            if (parts.Length == 2)
            {
               DateTime? startDate = parts[0].Trim().ToStartDate();
               DateTime? endDate = parts[1].Trim().ToEndDate();

               if (startDate.HasValue) judgebooks = judgebooks.Where(x => x.ReviewedAt >= startDate.Value);
               if (endDate.HasValue) judgebooks = judgebooks.Where(x => x.ReviewedAt <= endDate.Value);
            }
         }
         if (!string.IsNullOrEmpty(judgeDate))
         {
            string[] parts_j = judgeDate.Split(new string[] { "~" }, StringSplitOptions.None);
            if (parts_j.Length == 2)
            {
               int startNum = parts_j[0].Trim().Replace("-", "").ToInt();
               int endNum = parts_j[1].Trim().Replace("-", "").ToInt();

               if (startNum.IsValidRocDate()) judgebooks = judgebooks.Where(x => x.JudgeDate >= startNum);
               if (endNum.IsValidRocDate()) judgebooks = judgebooks.Where(x => x.JudgeDate <= endNum);
            }
         }

         var models = new List<JudgebookFileReportItem>();
         if (judgebooks.IsNullOrEmpty()) return models;

         var downloadRecords = await _judgebooksService.FetchDownloadRecordsAsync(judgebooks.Select(x => x.Id).ToList());
         judgebooks = judgebooks.GetOrdered();
         foreach (var entity in judgebooks)
         {
            var downloads = downloadRecords.HasItems() ? downloadRecords.Where(x => x.EntityId == entity.Id.ToString()).ToList() : null;
            var downloadViews = await downloads.MapViewModelListAsync(_usersService, _mapper);
            var model = entity.MapReportItem(_mapper);
            model.ModifyRecords = downloadViews;
            models.Add(model);
         }

         return models;
      }

      [HttpPost("reports")]
      public async Task<IActionResult> Reports(JudgebookReportsRequest request)
      {
         if (!CanReview()) return Forbid();
         
         if (request.Ids.IsNullOrEmpty())
         {
            ModelState.AddModelError("ids", "錯誤的ids");
            return BadRequest(ModelState);
         }
         string includes = "type";
         var entryList = await _judgebooksService.FetchAsync(request.Ids, includes);
         var items = entryList.MapReportItemList(_mapper);

         var model = new JudgebookFileReportModel(ReportTitle, request, items);
         var doc = new JudgebookFileReportDocument(model);

         byte[] bytes = doc.GeneratePdf();
         return Ok(new BaseFileView($"{_judgebookSettings.Title}報表{DateTime.Now.ToShortDateString()}", bytes));
      }

      [HttpGet("review/{ids}")]
      public async Task<IActionResult> Review(string ids)
      {
         if (!CanReview()) return Forbid();

         var idLists = ids.SplitToIntList(',');
         if (idLists.IsNullOrEmpty())
         {
            ModelState.AddModelError("ids", "錯誤的ids");
            return BadRequest(ModelState);
         }

         var entryList = await _judgebooksService.FetchAsync(idLists);
         return Ok(entryList.MapViewModelList(_mapper));
      }

      [HttpPost("review")]
      public async Task<IActionResult> Review([FromBody] IEnumerable<JudgebookReviewRequest> models)
      {
         if (!CanReview()) return Forbid();

         var idLists = models.Select(x => x.Id).ToList();
         var entryList = await _judgebooksService.FetchAsync(idLists);
         if (entryList.IsNullOrEmpty() || idLists.Count() != entryList.Count())
         {
            ModelState.AddModelError("ids", "錯誤的ids");
         }

         
         foreach (var model in models)
         {
            var entry = entryList.FirstOrDefault(x => x.Id == model.Id);
            model.SetValuesTo(entry!);
         }

         string ip = RemoteIpAddress;
         await _judgebooksService.ReviewRangeAsync(entryList, User.Id(), ip);

         return NoContent();
      }


      [HttpDelete("{id}")]
      public async Task<IActionResult> Remove(int id)
      {
         string include = "type";
         var entity = await _judgebooksService.GetByIdAsync(id, include);
         if (entity == null) return NotFound();

         if (!CanEdit(entity)) return Forbid();

         string ip = RemoteIpAddress;

         entity.Removed = true;
         MoveFile(entity, removed: true);

         await _judgebooksService.UpdateAsync(entity, ip);
         return NoContent();
      }
      string MoveFile(JudgebookFile entity, bool removed)
      {
         string sourceFolder = entity.DirectoryPath;
         string sourceFileName = entity.FileName;

         string destFolder = removed ? REMOVED : entity.CourtType;
         string destFileName = entity.CreateFileName() + entity.Ext;

         try
         {
            string destPath = _fileStoragesService.Move(sourceFolder, sourceFileName, destFolder, destFileName);
            return destPath;
         }
         catch (Exception ex)
         {
            if (ex is FileNotExistException)
            {
               throw new FileNotExistException(entity, (ex as FileNotExistException)!.Path);
            }
            else if (ex is MoveFileFailedException)
            {
               throw new MoveFileFailedException(entity, (ex as MoveFileFailedException)!.SourcePath, (ex as MoveFileFailedException)!.DestPath);
            }
            throw;
         }
      }
      
      async Task<Dictionary<string, string>> ValidateModelAsync(JudgebookFile model)
      {
         var errors = model.Validate();
         
         if (model.JudgeDate > 0)
         {
            if (!model.JudgeDate.IsValidRocDate()) errors.Add("judgeDate", "錯誤的judgeDate");
         }
         else
         {
            if (!_judgebookSettings.AllowEmptyJudgeDate) errors.Add("judgeDate", "錯誤的judgeDate");
         }
         if (string.IsNullOrEmpty(model.FileNumber))
         {
            if (!_judgebookSettings.AllowEmptyFileNumber) errors.Add("fileNumber", "錯誤的fileNumber");
         }
         else
         {
            if (!JudgebookFile.CheckFileNumber(model.FileNumber)) errors.Add("fileNumber", "錯誤的fileNumber");
         }

         if (_judgebookSettings.NoSameCaseEntries)
         {
            var sameCaseEntries = await _judgebooksService.FetchSameCaseEntriesAsync(model);
            if (sameCaseEntries.HasItems()) errors.Add("duplicate", "此年度字號案號重複了");
         }

         return errors;
      }

      Dictionary<string, string> ValidateFile(IFormFile file)
      {
         var errors = new Dictionary<string, string>();
         if (file == null) errors.Add("file", "必須上傳檔案");
         else
         {
            long fileSize = file!.Length; // Size of the file in bytes
            long maxFileSize = 250 * 1024 * 1024; // 250 MB (in bytes)
            if (fileSize > maxFileSize) errors.Add("file", "檔案過大");
         }

         return errors;
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      {
         if (disposing)
         {
            // Dispose the _ftpService when the controller is disposed
            _fileStoragesService.Dispose();
         }
      }
   }


}