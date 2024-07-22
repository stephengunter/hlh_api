using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Web.Models;
using ApplicationCore.Services.Files;
using Azure.Core;
using Ardalis.Specification;
using ApplicationCore.Exceptions;
using ApplicationCore.Authorization;

namespace Web.Controllers.Api;

public class AttachmentsController : BaseApiController
{
   private readonly IWebHostEnvironment _environment;
   private readonly AttachmentSettings _attachmentSettings;
   private readonly IFileStoragesService _fileStoragesService;
   private readonly IAttachmentService _attachmentService;
   private readonly IMapper _mapper;

   public AttachmentsController(IWebHostEnvironment environment, IOptions<AttachmentSettings> attachmentSettings,
      IAttachmentService attachmentService, IMapper mapper)
   {
      _environment = environment;
      _attachmentSettings = attachmentSettings.Value;
      _attachmentService = attachmentService;

      _mapper = mapper;

      if (String.IsNullOrEmpty(_attachmentSettings.Host))
      {
         _fileStoragesService = new LocalStoragesService(_attachmentSettings.Directory);
      }
      else
      {
         _fileStoragesService = new FtpStoragesService(_attachmentSettings.Host, _attachmentSettings.UserName,
         _attachmentSettings.Password, _attachmentSettings.Directory);
      }
   }

   [HttpPost("temp")]
   public async Task<ActionResult<IEnumerable<FileViewModel>>> Temp([FromForm] FilesRequest request)
   {
      if (request.Files.Count < 1)
      {
         ModelState.AddModelError("files", "必須上傳檔案");
         return BadRequest(ModelState);
      }

      string date = DateTime.Today.ToDateNumber().ToString();
      string folder = GetTempPath(_environment, date);
      if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

      var fileViewModels = new List<FileViewModel>();

      foreach (var file in request.Files)
      {
         var uuid = Guid.NewGuid().ToString();
         var extension = Path.GetExtension(file.FileName);
         var newFileName =  uuid + extension;


         var filePath = Path.Combine(folder, newFileName);

         using (var stream = new FileStream(filePath, FileMode.Create))
         {
            await file.CopyToAsync(stream);
         }

         fileViewModels.Add(new FileViewModel { Name = file.FileName, Path = $"{date}/{newFileName}" });
      }
      
      return fileViewModels;
   }

   [HttpPost]
   public async Task<ActionResult<AttachmentViewModel>> Store([FromForm] AttachmentCreateForm form)
   {
      if (form.File == null)
      {
         ModelState.AddModelError("file", "必須上傳檔案");
         return BadRequest(ModelState);
      }

      string filePath = SaveFile(form.File!);
      var entity = new Attachment()
      {
         Title = form.Title,
         Description = form.Title,
         PostType = form.PostType
      };
      if (form.PostId > 0) entity.PostId = form.PostId;

      entity.OriFileName = form.File.Name;
      entity.DirectoryPath = Path.GetDirectoryName(filePath)!;
      entity.FileName = Path.GetFileName(filePath);
      entity.Ext = Path.GetExtension(form.File!.FileName);
      entity.FileSize = form.File!.Length;
      entity.SetCreated(User.Id());

      entity = await _attachmentService.CreateAsync(entity);
      return entity.MapViewModel(_mapper);
   }

   string SaveFile(IFormFile file)
   {
      string folderPath = Path.Combine(_attachmentSettings.Directory, DateTime.Today.ToDateNumber().ToString());
      string ext = Path.GetExtension(file.FileName);
      string fileName = Guid.NewGuid().ToString() + ext;

      return _fileStoragesService.Create(file, folderPath, fileName);
   }

   [HttpGet("{id}")]
   public async Task<ActionResult> Get(int id)
   {
      var entity = await _attachmentService.GetByIdAsync(id);
      if (entity == null) return NotFound();

     // if (!CanDownload(entity)) return Forbid();

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

}