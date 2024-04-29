
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
using static System.Net.Mime.MediaTypeNames;

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

      string RemovedFolderPath => GetFolderPath("removed");

      [HttpGet]
      public async Task<ActionResult<JudgebookFilesAdminModel>> Index(string courtType = "", string year = "", string category = "", string num = "", int page = 1, int pageSize = 99)
      {
         var request = new JudgebookFilesAdminRequest(courtType, year, category, num, page, pageSize);

         var model = new JudgebookFilesAdminModel(request);

         var judgebooks = await _judgebooksService.FetchAllAsync();
         if (!String.IsNullOrEmpty(request.CourtType)) judgebooks = judgebooks.Where(x => x.CourtType == request.CourtType);
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

         var errors = await ValidateModelAsync(model);
         AddErrors(errors);

         if (!ModelState.IsValid) return BadRequest(ModelState);

         bool sameCase = entity.IsSameCase(model);

         entity = model.MapEntity(_mapper, User.Id(), entity);


         if (!sameCase)
         {
            string folderPath = GetFolderPath(entity.CourtType);
            string destPath = MoveFile(entity, folderPath);

            entity.FileName = Path.GetFileName(destPath);
            entity.DirectoryPath = folderPath;
         }

         await _judgebooksService.UpdateAsync(entity);

         

         return NoContent();
      }


      [HttpPost("upload")]
      public async Task<ActionResult> Upload([FromForm] List<JudgebookUploadRequest> models)
      {
         string courtType = models.FirstOrDefault()!.CourtType;
         string folderPath = GetFolderPath(courtType);

         var resultList = new List<JudgebookFileUploadResponse>();
         for (int i = 0; i < models.Count; i++)
         {
            var result = new JudgebookFileUploadResponse() { id = models[i].Id};
            var file = models[i].File;
            var entry = new JudgebookFile(courtType, models[i].Year, models[i].Category, models[i].Num, models[i].Ps, models[i].Type);

            var errors = await ValidateRequestAsync(entry, file);
            if (errors.Count > 0)
            {
               result.Errors = errors;
            }
            else
            {
               string ext = Path.GetExtension(file.FileName);
               string fileName = entry.CreateFileName() + ext;
               string filePath = FilesHelpers.GetUniqueFileName(folderPath, fileName);
               
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
               
               entry.FileName = Path.GetFileName(filePath);
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

         var model = entity.MapViewModel(_mapper, entity.FullPath);

         return Ok(model);
      }

      [HttpDelete("{id}")]
      public async Task<IActionResult> Remove(int id)
      {
         var entity = await _judgebooksService.GetByIdAsync(id);
         if (entity == null) return NotFound();
         MoveFile(entity, RemovedFolderPath);

         entity.Removed = true;
         await _judgebooksService.UpdateAsync(entity);

         return NoContent();
      }

      string MoveFile(JudgebookFile entity, string folderPath)
      {
         string sourcePath = entity.FullPath;
         if (!System.IO.File.Exists(sourcePath))
         {
            throw new FileNotExistException(entity, sourcePath);
         }
         
         string fileName = entity.CreateFileName() + entity.Ext;
         string destPath = FilesHelpers.GetUniqueFileName(folderPath, fileName);

         try
         {
            System.IO.File.Move(sourcePath, destPath);
         }
         catch (Exception ex)
         {
            throw new MoveFileFailedException(entity, sourcePath, destPath);
         }
         return destPath;
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
   }


}