
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

namespace Web.Controllers.Api.Files
{
   [Route("api/files/[controller]")]
   [Authorize(Policy = "JudgebookFiles")]
   public class JudgebooksController : BaseApiController, IDisposable
   {
      private readonly JudgebookFileSettings _judgebookSettings;
      private readonly IJudgebookFilesService _judgebooksService;
      private readonly IFileStoragesService _fileStoragesService;
      private readonly IMapper _mapper;
      private const string REMOVED = "removed";
      public JudgebooksController(IOptions<JudgebookFileSettings> judgebookSettings, IJudgebookFilesService judgebooksService,
         IMapper mapper)
      {
         _judgebookSettings = judgebookSettings.Value;
         _judgebooksService = judgebooksService;
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

      [HttpGet]
      public async Task<ActionResult<JudgebookFilesAdminModel>> Index(int reviewed = -1, int typeId = 0, string fileNumber = "", string courtType = "", string year = "", string category = "", string num = "", int page = 1, int pageSize = 50)
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
         string originType = "";
         int judgeDate = 0;
         var request = new JudgebookFilesAdminRequest(new JudgebookFile(type, judgeDate, fileNumber, originType, courtType, year, category, num), reviewed, page, pageSize);
         
         var actions = new List<string>();
         if (CanReview()) actions.Add(ActionsTypes.Review);
         var model = new JudgebookFilesAdminModel(request, actions);
        
         string include = "type";
         var judgebooks = type == null ? 
            await _judgebooksService.FetchAllAsync(include) 
            : await _judgebooksService.FetchAsync(type, include);


         if (request.Reviewed == 0) judgebooks = judgebooks.Where(x => x.Reviewed == false);
         else if (request.Reviewed == 1) judgebooks = judgebooks.Where(x => x.Reviewed == true);

         if (!String.IsNullOrEmpty(request.FileNumber)) judgebooks = judgebooks.Where(x => x.FileNumber == request.FileNumber);
         if (!String.IsNullOrEmpty(request.CourtType)) judgebooks = judgebooks.Where(x => x.CourtType == request.CourtType);
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

         if (!CanEdit(entity)) return Forbid();

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

         var model = entity.MapViewModel(_mapper, bytes);
         return Ok(model);
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