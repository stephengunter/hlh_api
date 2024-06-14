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

namespace Web.Controllers.Api;

public class AttachmentsController : BaseApiController
{
   private readonly IWebHostEnvironment _environment;
   public AttachmentsController(IWebHostEnvironment environment)
   {
      _environment = environment;
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

}