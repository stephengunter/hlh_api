using ApplicationCore.Helpers;
using ApplicationCore.Helpers.Criminals;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;
using ApplicationCore.Services.Criminals;
using ApplicationCore.Views.Criminals;
using Web.Models.Criminals;
using ApplicationCore.Models.Criminals;
using Infrastructure.Paging;

namespace Web.Controllers.Admin.Criminals;

[Route("admin/criminals/[controller]")]
public class RecordsController : BaseAdminController
{
   
   private readonly IMapper _mapper;
   private readonly ICriminalFetchRecordService _service;
   
   public RecordsController(ICriminalFetchRecordService service, IMapper mapper)
   {
      _service = service;
      _mapper = mapper;
   }
   [HttpGet("init")]
   public async Task<ActionResult<CriminalFetchRecordsIndexModel>> Init()
   {
      var today = DateTime.Today;
      int year = today.Year - 1911;
      int month = today.Month - 1;

      var minYear = await _service.FindMinYearAsync();
      if (!minYear.HasValue) minYear = year - 1;
      var years = new List<int>();
      for (int i = minYear.Value; i <= year; i++)
      {
         years.Add(i);
      }
      int page = 1;
      int pageSize = 10;
      var request = new CriminalFetchRecordsFetchRequest(year, month, page, pageSize);

      return new CriminalFetchRecordsIndexModel(years, request);
   }

   [HttpGet]
   public async Task<ActionResult<PagedList<CriminalFetchRecord, CriminalFetchRecordView>>> Index(int year, int month, int page = 1, int pageSize = 10)
   {
      var records = await _service.FetchAsync(year, month);
      records = records.GetOrdered();

      return records.GetPagedList(_mapper, page, pageSize);
   }

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] CriminalFetchRecordsAddRequest request)
   {
      int year = request.Year;
      int month = request.Month;
      var list = await _service.FetchAsync(year, month);
      if (list.HasItems())
      {
         ModelState.AddModelError("month", "此年度月份已經有資料");
         return BadRequest(ModelState);
      }

      var records = request.Records.MapEntityList(_mapper);
      await _service.AddRangeAsync(records);
      return Ok();

   }

   [HttpPost("upload")]
   public async Task<ActionResult<ICollection<CriminalFetchRecordView>>> Upload([FromForm] CriminalFetchRecordsUploadRequest request)
   {
      var file = request.File;
      var errors = ValidateFile(file!);
      AddErrors(errors);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var records = new List<CriminalFetchRecord>();
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

            for (int row = 2; row <= rowCount; row++)
            {
               string code = worksheet.Cells[row, 3].Text;
               if (string.IsNullOrEmpty(code)) continue;

               string name = worksheet.Cells[row, 4].Text;
               string queryKind = worksheet.Cells[row, 5].Text;
               string date = worksheet.Cells[row, 6].Text;
               var parts = date.ResolveDate();
               string time = worksheet.Cells[row, 7].Text;
               string kind = worksheet.Cells[row, 8].Text;
               string query = worksheet.Cells[row, 9].Text;
               string result = worksheet.Cells[row, 10].Text;
               string ip = worksheet.Cells[row, 11].Text;

               
               var record = new CriminalFetchRecord()
               {
                  UserCode = code,
                  UserName = name,
                  QueryKind = queryKind,
                  Query = query,
                  Year = parts[0],
                  Month = parts[1],
                  Day = parts[2],
                  Time = time,
                  DataKind = kind,
                  IP = ip,
                  Result = result  
               };
               records.Add(record);
            }
         }
      }
      return records.MapViewModelList(_mapper);
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