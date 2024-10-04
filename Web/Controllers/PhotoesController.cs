using ApplicationCore.Services;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Helpers;
using Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Services.Files;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.Exceptions;

namespace Web.Controllers;

[EnableCors("Global")]
[Authorize]
public class PhotoesController : BaseController
{
   private readonly IWebHostEnvironment _environment;
   private readonly AttachmentSettings _attachmentSettings;
   private readonly IFileStoragesService _fileStoragesService;
   private readonly IAttachmentService _attachmentService;
   private readonly IMapper _mapper;

   public PhotoesController(IWebHostEnvironment environment, IOptions<AttachmentSettings> attachmentSettings,
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

   [HttpGet("{id}")]
   public async Task<ActionResult> Get(int id)
   {
      var entity = await _attachmentService.GetByIdAsync(id);
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

      return File(bytes, "image/jpeg");
   }

   private async Task<ActionResult> SendOriginalImageAsync(string imgSourcePath)
   {
      byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(imgSourcePath);
      return File(imageBytes, "image/jpeg");
   }

   private string ValidateRequest(string name)
   {
      // Check if the file exists in the server's file system
      var filePath = Path.Combine(_environment.WebRootPath, "images", name); // Example path
      return System.IO.File.Exists(filePath) ? filePath : string.Empty;
   }

}