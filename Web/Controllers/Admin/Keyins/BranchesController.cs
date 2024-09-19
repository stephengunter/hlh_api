using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using ApplicationCore.Helpers.Keyin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;
using ApplicationCore.Services.Keyin;
using ApplicationCore.Views.Keyin;
using Web.Models.Keyin;
using ApplicationCore.Models.Keyin;
using Infrastructure.Views;
using QuestPDF.Fluent;

namespace Web.Controllers.Admin.Keyins;

[Route("admin/keyins/[controller]")]
public class BranchesController : BaseAdminController
{
   private readonly CompanySettings _companySettings;
   private readonly IMapper _mapper;
   private readonly IBranchesService _branchesService;
   private readonly IBranchRecordService _branchRecordService;


   
   public BranchesController(IOptions<CompanySettings> companySettings, IMapper mapper, 
      IBranchesService branchesService, IBranchRecordService branchRecordService)
   {
      _companySettings = companySettings.Value;
      _branchesService = branchesService;
      _mapper = mapper;
      _branchRecordService = branchRecordService;
   }
   [HttpGet("init")]
   public async Task<ActionResult<BranchRecordsIndexModel>> Init()
   {
      var today = DateTime.Today;
      int year = today.Year - 1911;
      int month = today.Month - 1;

      var minYear = await _branchRecordService.FindMinYearAsync();
      if (!minYear.HasValue) minYear = year - 1;
      var years = new List<int>();
      for (int i = minYear.Value; i <= year; i++)
      {
         years.Add(i);
      }

      var request = new BranchRecordsFetchRequest(year, month);
      var branches = await _branchesService.FetchAsync();

      return new BranchRecordsIndexModel(years, request, branches.MapViewModelList(_mapper));
   }
   [HttpGet]
   public async Task<ActionResult<ICollection<BranchRecordView>>> Index(int year, int month)
   {
      var records = await _branchRecordService.FetchAsync(year, month);
      records = records.OrderBy(r => r.Rank).OrderBy(r => r.Id).ToList();

      var branches = await _branchesService.FetchAsync();
      var views = new List<BranchRecordView>();
      foreach (var record in records)
      {
         record.Branch = branches.FirstOrDefault(x => x.Id == record.BranchId);
         views.Add(record.MapViewModel(_mapper));
      }
      return views;
   }

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] BranchRecordsAddRequest request)
   {
      int year = request.Year;
      int month = request.Month;
      foreach (var item in request.Records)
      {
         var entity = await _branchRecordService.FindAsync(new Branch { Id = item.BranchId }, year, month);
         if (entity != null)
         {
            entity.Score = item.Score;
            entity.Rank = item.Rank;
            entity.AbsentRate = item.AbsentRate;
            await _branchRecordService.UpdateAsync(entity);
         }
         else
         {
            item.Month = month;
            item.Year = year;

            entity = await _branchRecordService.CreateAsync(item.MapEntity(_mapper));
         }

         var prevMonth = month - 1;
         var prevRecord = await _branchRecordService.FindAsync(new Branch { Id = item.BranchId },
                                                               prevMonth < 1 ? (year - 1) : year,
                                                               prevMonth < 1 ? 12 : prevMonth
                                                               );
         if (prevRecord != null && entity.Score != 0 && prevRecord.Score != 0)
         {
            double increaseRate = ((double)(entity.Score - prevRecord.Score) / prevRecord.Score) * 100;
            entity.Diff = Math.Round(increaseRate, 2).ToString();
            await _branchRecordService.UpdateAsync(entity);
         }

         //var nextMonth = month + 1;

         //var nextRecord = await _branchRecordService.FindAsync(new Branch { Id = item.BranchId }, 
         //                                                      nextMonth > 12 ? (year + 1) : year,
         //                                                      nextMonth > 12 ? 1 : nextMonth
         //                                                      );
         //if (nextRecord != null && entity.Score != 0 && nextRecord.Score != 0)
         //{ 
         //   double increaseRate = ((double)(nextRecord.Score - entity.Score) / entity.Score) * 100;
         //   nextRecord.Diff = Math.Round(increaseRate, 2).ToString();
         //   await _branchRecordService.UpdateAsync(nextRecord);
         //}
      }
      return Ok();

   }

   [HttpPost("upload")]
   public async Task<ActionResult<ICollection<BranchRecordView>>> Upload([FromForm] BranchRecordsUploadRequest request)
   {
      var file = request.File;
      var errors = ValidateFile(file!);
      AddErrors(errors);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var records = new List<BranchRecord>();
      using (var stream = new MemoryStream())
      {
         await file!.CopyToAsync(stream);
         using (var package = new OfficeOpenXml.ExcelPackage(stream))
         {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault(); // Get the first worksheet
            if (worksheet == null)
            {
               ModelState.AddModelError("file", "無法讀取工作表");
               return BadRequest(ModelState);
            }

            var rowCount = worksheet.Dimension.Rows;
            var colCount = worksheet.Dimension.Columns;

            for (int row = 1; row <= rowCount; row++)
            {
               int rank = worksheet.Cells[row, 1].Text.ToInt();
               string branchTitle = worksheet.Cells[row, 2].Text;
               int score = worksheet.Cells[row, 3].Text.ToInt();
               string absentRate = worksheet.Cells[row, 4].Text;
               if (string.IsNullOrEmpty(branchTitle)) continue;
               if (score < 1) continue;

               var branch = await _branchesService.FindByTitleAsync(branchTitle);
               if(branch == null)
               {
                  branch = await _branchesService.CreateAsync(new Branch
                  { 
                     Title = branchTitle
                  });
               }
               var record = new BranchRecord()
               {
                  Rank = rank,
                  AbsentRate = absentRate,
                  Score = score,
                  BranchId = branch.Id,
                  Branch = branch
               };
               records.Add(record);
            }
         }
      }
      return records.MapViewModelList(_mapper);
   }
   [HttpPost("reports")]
   public async Task<IActionResult> Reports(BranchRecordReportRequest request)
   {
      var records = await _branchRecordService.FetchAsync(request.Year, request.Month);
      records = records.OrderBy(r => r.Rank).OrderBy(r => r.Id).ToList();

      var branches = await _branchesService.FetchAsync();
      var views = new List<BranchRecordView>();
      foreach (var record in records)
      {
         record.Branch = branches.FirstOrDefault(x => x.Id == record.BranchId);
         views.Add(record.MapViewModel(_mapper));
      }
      var items = views.MapReportItemList();
      string title = $"{request.Year}年{request.Month}月 各法院書記官聽打成績";
      var model = new BranchRecordReportModel(title, request, items);
      var doc = new BranchRecordReportDocument(model);

      byte[] bytes = doc.GeneratePdf();
      return Ok(new BaseFileView(title, bytes));
   }
   Dictionary<string, string> ValidateFile(IFormFile file)
   {
      var errors = new Dictionary<string, string>();
      if (file == null)
      {
         errors.Add("file", "必須上傳檔案");
         return errors;
      }
      else
      {
         // Check file extension (for Excel files)
         var allowedExtensions = new[] { ".xlsx", ".xls" };
         var extension = Path.GetExtension(file.FileName).ToLower();

         if (!allowedExtensions.Contains(extension))
         {
            errors.Add("file", "只接受 Excel 檔案 (.xlsx, .xls)");
            return errors;
         }

         // Check MIME type for Excel
         if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
             file.ContentType != "application/vnd.ms-excel")
         {
            errors.Add("file", "檔案類型必須是 Excel (.xlsx, .xls)");
            return errors;
         }

         return errors;
      }
   }

}