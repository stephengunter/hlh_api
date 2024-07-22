using ApplicationCore.Models;
using ApplicationCore.Services.Files;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using AutoMapper;
using Web.Controllers;
using ApplicationCore.Exceptions;
using Infrastructure.Helpers;

namespace Web.Helpers;
public static class ControllerHelpers
{
   public static void InitFileStoragesService(this BaseController controller, IFileStoragesService fileStoragesService, AttachmentSettings attachmentSettings)
   {
      if (String.IsNullOrEmpty(attachmentSettings.Host))
      {
         fileStoragesService = new LocalStoragesService(attachmentSettings.Directory);
      }
      else
      {
         fileStoragesService = new FtpStoragesService(attachmentSettings.Host, attachmentSettings.UserName,
         attachmentSettings.Password, attachmentSettings.Directory);
      }
   }

   public static Attachment SaveAttamentFile(this BaseController controller, IFormFile file, IFileStoragesService fileStoragesService)
   {
      string filePath = SaveFile(file, fileStoragesService);
      var attchment = new Attachment();
      attchment.FileName = Path.GetFileName(filePath);
      attchment.Ext = Path.GetExtension(file.FileName);

      attchment.Host = fileStoragesService.Host;
      attchment.DirectoryPath = Path.GetDirectoryName(filePath)!;

      attchment.FileSize = file.Length;

      return attchment;
   }

   static string SaveFile(IFormFile file, IFileStoragesService fileStoragesService)
   {
      string folderPath = DateTime.Today.GetDateString();
      string ext = Path.GetExtension(file.FileName);
      string fileName = Guid.NewGuid().ToString() + ext;

      string path = fileStoragesService.Create(file, folderPath, fileName);
      byte[] bytes = fileStoragesService.GetBytes(folderPath, fileName);
      if (bytes == null) throw new UploadFileFailedException(folderPath, fileName);

      return path;
   }
}

