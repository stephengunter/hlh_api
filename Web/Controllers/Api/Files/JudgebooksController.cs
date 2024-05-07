
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
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Infrastructure.Entities;
using System.IO;

namespace Web.Controllers.Api.Files
{
   [Route("api/files/[controller]")]
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
      public async Task<ActionResult<JudgebookFilesAdminModel>> Index(int typeId, string fileNumber = "", string courtType = "", string year = "", string category = "", string num = "", int page = 1, int pageSize = 99)
      {
         var type = await _judgebooksService.GetTypeByIdAsync(typeId);
         if (type == null)
         {
            ModelState.AddModelError("typeId", $"Type 不存在 Id: ${typeId}");
            return BadRequest(ModelState);
         }
         var request = new JudgebookFilesAdminRequest(typeId, fileNumber,courtType, year, category, num, page, pageSize);

         var model = new JudgebookFilesAdminModel(request);
         string include = "type";
         var judgebooks = await _judgebooksService.FetchAsync(type, include);
         if (!String.IsNullOrEmpty(request.FileNumber)) judgebooks = judgebooks.Where(x => x.FileNumber == request.FileNumber);
         if (!String.IsNullOrEmpty(request.CourtType)) judgebooks = judgebooks.Where(x => x.CourtType == request.CourtType);
         if (!String.IsNullOrEmpty(request.Year)) judgebooks = judgebooks.Where(x => x.Year == request.Year);
         if (!String.IsNullOrEmpty(request.Category)) judgebooks = judgebooks.Where(x => x.Category == request.Category);
         if (!String.IsNullOrEmpty(request.Num)) judgebooks = judgebooks.Where(x => x.Num == request.Num);


         model.PagedList = judgebooks.GetOrdered().GetPagedList(_mapper, page, pageSize);

         return model;
      }

      [HttpGet("types")]
      public async Task<ActionResult<IEnumerable<JudgebookTypeViewModel>>> Types()
      {
         var types = await _judgebooksService.FetchTypesAsync();
         return types.MapViewModelList(_mapper);
      }


      [HttpGet("edit/{id}")]
      public async Task<ActionResult<JudgebookFileViewModel>> Edit(int id)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();


         return entity.MapViewModel(_mapper);
      }

      [HttpPut("{id}")]
      public async Task<ActionResult> Update(int id, [FromBody] JudgebookFileViewModel model)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();

         var errors = await ValidateModelAsync(model);
         AddErrors(errors);

         if (!ModelState.IsValid) return BadRequest(ModelState);

         bool sameCase = entity.IsSameCase(model);

         entity = model.MapEntity(_mapper, User.Id(), entity);


         if (!sameCase)
         {
            string destPath = MoveFile(entity);

            entity.FileName = Path.GetFileName(destPath);
            entity.DirectoryPath = Path.GetDirectoryName(destPath)!;
         }

         await _judgebooksService.UpdateAsync(entity);



         return NoContent();
      }

      string SaveFile(IFormFile file, JudgebookFile entry)
      {
         string folderPath = entry.CourtType;
         string ext = Path.GetExtension(file.FileName);
         string fileName = entry.CreateFileName() + ext;   //$"{entry.Year}_{entry.Category}_{entry.Num}";

         return _fileStoragesService.Create(file, folderPath, fileName);
      }

      async Task<JudgebookFileUploadResponse> AddOneAsync(JudgebookUploadRequest request)
      {
         var result = new JudgebookFileUploadResponse() { id = request.Id };
         var file = request.File;
         var entry = new JudgebookFile(request.TypeId, request.FileNumber, request.CourtType, request.Year, request.Category, request.Num, request.Ps);
         var errors = await ValidateRequestAsync(entry, file);
         if (errors.Count > 0)
         {
            result.Errors = errors;
            return result;
         }

         try
         {
            string filePath = SaveFile(file, entry);
            entry.FileName = Path.GetFileName(filePath);
            entry.Ext = Path.GetExtension(file.FileName);

            entry.Host = _judgebookSettings.Host;
            entry.DirectoryPath = Path.GetDirectoryName(filePath)!;

            entry.FileSize = file!.Length;
            entry.SetCreated(User.Id());

            entry = await _judgebooksService.CreateAsync(entry);
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
            errors.Add("file", $"檔案上傳失敗. {ex.Message}");
            result.Errors = errors;
            return result;
         }


      }


      [HttpPost("upload")]
      public async Task<ActionResult> Upload([FromForm] List<JudgebookUploadRequest> models)
      {
         var resultList = new List<JudgebookFileUploadResponse>();
         for (int i = 0; i < models.Count; i++)
         {
            var result = await AddOneAsync(models[i]);
            resultList.Add(result);
         }

         return Ok(resultList);
      }

      [HttpGet("download/{id}")]
      public async Task<ActionResult> Download(int id)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();

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


      [HttpDelete("{id}")]
      public async Task<IActionResult> Remove(int id)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();

         entity.Removed = true;
         MoveFile(entity);

         await _judgebooksService.UpdateAsync(entity);
         return NoContent();
      }

      string MoveFile(JudgebookFile entity)
      {
         string sourceFolder = entity.DirectoryPath;
         string sourceFileName = entity.FileName;

         string destFolder = entity.Removed ? REMOVED : entity.CourtType;
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

      async Task<Dictionary<string, string>> ValidateModelAsync(IJudgebookFile model)
      {
         var errors = new Dictionary<string, string>();
         if (String.IsNullOrEmpty(model.CourtType)) errors.Add("courtType", "錯誤的CourtType");

         if (String.IsNullOrEmpty(model.Year)) errors.Add("year", "必須填寫年度");
         if (String.IsNullOrEmpty(model.Category)) errors.Add("category", "必須填寫字號");
         if (String.IsNullOrEmpty(model.Num)) errors.Add("num", "必須填寫案號");


         //var exist = await _judgebooksService.FindAsync(entity);
         //if (exist != null && exist.Id != entity.Id) errors.Add("duplicate", "此年度字號案號重複了");

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

      async Task<Dictionary<string, string>> ValidateRequestAsync(JudgebookFile model, IFormFile file)
      {
         var modelErrors = await ValidateModelAsync(model);
         var fileErrors = ValidateFile(file);

         Dictionary<string, string> errors = modelErrors.Concat(fileErrors)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

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