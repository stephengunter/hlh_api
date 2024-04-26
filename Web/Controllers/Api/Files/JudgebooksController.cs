
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers.Files;
using AutoMapper;
using Web.Models.Files;
using System.IO;
using Microsoft.Extensions.Options;
using ApplicationCore.Exceptions;
using ApplicationCore.Consts;
using ApplicationCore.Settings.Files;
using ApplicationCore.Services.Files;
using ApplicationCore.Views.Files;
using ApplicationCore.Models.Files;
using System;
using Web.Models;
using Ardalis.Specification;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Google.Apis.Util;
using ApplicationCore.Views;
using System.Collections.Generic;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;

namespace Web.Controllers.Api.Files
{
   [Route("api/files/[controller]")]
   public class JudgebooksController : BaseApiController
   {
      private readonly JudgebookFileSettings _judgebookSettings;
      private readonly IJudgebookFilesService _judgebooksService;
      private readonly IMapper _mapper;

      public JudgebooksController(IOptions<JudgebookFileSettings> judgebookSettings, IJudgebookFilesService judgebooksService, IMapper mapper)
      {
         _judgebookSettings = judgebookSettings.Value;
         _judgebooksService = judgebooksService;
         _mapper = mapper;
      }

      string GetFolderPath(string key)
      {
         if (String.IsNullOrEmpty(_judgebookSettings.Directory))
         {
            throw new SettingsException("JudgebookFileSettings.Directory");
         }
         string folderPath = Path.Combine(_judgebookSettings.Directory, key);
         if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
         return folderPath;
      }

      [HttpGet]
      public async Task<ActionResult<JudgebookFilesAdminModel>> Index(string year = "", string category = "", string num = "", int page = 1, int pageSize = 99)
      {
         var request = new JudgebookFilesAdminRequest(year, category, num, page, pageSize);

         var model = new JudgebookFilesAdminModel(request);

         var judgebooks = await _judgebooksService.FetchAllAsync();

         if (!String.IsNullOrEmpty(request.Year)) judgebooks = judgebooks.Where(x => x.Year == request.Year);
         if (!String.IsNullOrEmpty(request.Category)) judgebooks = judgebooks.Where(x => x.Category == request.Category);
         if (!String.IsNullOrEmpty(request.Num)) judgebooks = judgebooks.Where(x => x.Num == request.Num);


         model.PagedList = judgebooks.GetOrdered().GetPagedList(_mapper, page, pageSize);

         return model;
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

         entity = model.MapEntity(_mapper, User.Id(), entity);

         var errors = await ValidateEntityAsync(entity);
         AddErrors(errors);

         //errors = ValidateFile(entity);
         //AddErrors(errors);
         if (!ModelState.IsValid) return BadRequest(ModelState);

         await _judgebooksService.UpdateAsync(entity);

         return NoContent();
      }


      [HttpPost("upload")]
      public async Task<ActionResult> Upload([FromForm] List<JudgebookUploadRequest> models)
      {
         string courtType = JudgeCourtTypes.H;
         string folderPath = GetFolderPath(courtType);

         var resultList = new List<JudgebookFileUploadResponse>();
         for (int i = 0; i < models.Count; i++)
         {
            var result = new JudgebookFileUploadResponse() { id = models[i].Id};
            var file = models[i].File;
            var entry = new JudgebookFile(models[i].Year, models[i].Category, models[i].Num, models[i].Ps, models[i].Type);

            var errors = await ValidateRequestAsync(entry, file);
            if (errors.Count > 0)
            {
               result.Errors = errors;
            }
            else
            {
               string ext = Path.GetExtension(file.FileName);
               string fileName = $"{entry.Year}_{entry.Category}_{entry.Num}" + ext;
               string filePath = GetUniqueFileName(folderPath, fileName);
               
               try
               {
                  using (var stream = new FileStream(filePath, FileMode.Create))
                  {
                     await file.CopyToAsync(stream);
                  }
               }
               catch (Exception ex)
               {
                  errors = new Dictionary<string, string>();
                  errors.Add("file", $"檔案上傳失敗. {ex.Message}");
                  result.Errors = errors;

                  continue;
               }

               entry.FileName = fileName;
               entry.Ext = ext;
               entry.FileSize = file!.Length;
               entry.Host = _judgebookSettings.Host;
               entry.DirectoryPath = folderPath;
               entry.SetCreated(User.Id());

               entry = await _judgebooksService.CreateAsync(entry);
               if (entry == null)
               {
                  errors = new Dictionary<string, string>();
                  errors.Add("judgebook", $"{i},{file.FileName}");
                  result.Errors = errors;
               }
               else
               {
                  result.Model = entry.MapViewModel(_mapper);
               }


            }

            resultList.Add(result);
         }

         return Ok(resultList);
      }

      [HttpGet("download/{id}")]
      public async Task<ActionResult> Download(int id)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();

         if (!System.IO.File.Exists(entity.FullPath))
         {
            throw new FileNotExistException(entity, entity.FullPath);
         }

         byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(entity.FullPath);

         return File(fileBytes, "application/pdf", entity.FileName);
      }

      [HttpDelete("{id}")]
      public async Task<IActionResult> Remove(int id)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();

         entity.Removed = true;
         await _judgebooksService.UpdateAsync(entity);

         return NoContent();
      }


      async Task<Dictionary<string, string>> ValidateEntityAsync(JudgebookFile entity)
      {
         var errors = new Dictionary<string, string>();
         if (String.IsNullOrEmpty(entity.Year)) errors.Add("year", "必須填寫年度");
         if (String.IsNullOrEmpty(entity.Category)) errors.Add("category", "必須填寫字號");
         if (String.IsNullOrEmpty(entity.Num)) errors.Add("num", "必須填寫案號");


         var exist = await _judgebooksService.FindAsync(entity);
         if (exist != null && exist.Id != entity.Id) errors.Add("duplicate", "此年度字號案號重複了");

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
         var errors = new Dictionary<string, string>();

         if (String.IsNullOrEmpty(model.Year)) errors.Add("year", "必須填寫年度");
         if (String.IsNullOrEmpty(model.Category)) errors.Add("category", "必須填寫字號");
         if (String.IsNullOrEmpty(model.Num)) errors.Add("num", "必須填寫案號");

         if (file == null) errors.Add("file", "必須上傳檔案");
         else
         {
            long fileSize = file!.Length; // Size of the file in bytes
            long maxFileSize = 250 * 1024 * 1024; // 250 MB (in bytes)
            if (fileSize > maxFileSize) errors.Add("file", "檔案過大");
         }

         var exist = await _judgebooksService.FindAsync(model);
         if (exist != null) errors.Add("duplicate", "此年度字號案號重複了");

         return errors;
      }

      string GetUniqueFileName(string folderPath, string fileName)
      {
         string extension = Path.GetExtension(fileName);
         string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
         string filePath = Path.Combine(folderPath, fileName);
         int count = 1;
         while (System.IO.File.Exists(filePath))
         {
            string newFileName = $"{fileNameWithoutExtension}_({count}){extension}";
            filePath = Path.Combine(folderPath, newFileName);
            count++;
         }
         return filePath;
      }


   }


}