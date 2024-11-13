using ApplicationCore.Helpers;
using ApplicationCore.Helpers.Fetches;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;
using Web.Models.Fetches;
using ApplicationCore.Models.Fetches;
using ApplicationCore.Services.Fetches;
using ApplicationCore.Consts;
using ApplicationCore.Views.Fetches;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Infrastructure.Views;
using QuestPDF.Fluent;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Infrastructure.Consts;

namespace Web.Controllers.Admin.Fetches;

[Route("admin/fetches/[controller]")]
public class RecordsController : BaseAdminController
{
   private readonly IWebHostEnvironment _environment;
   private readonly AppSettings _appSettings;
   private readonly IMapper _mapper;
   private readonly IFetchesSystemService _systemService;
   private readonly IFetchesRecordService _service;
   private Dictionary<string, string> _KeysTitles = new Dictionary<string, string>()
   {
       { "Name", "姓名" },{ "Identifier", "識別碼" },
       { "CourtType", "系統別" },{ "CaseNumber", "查詢案號" },
       { "QueryTime", "查詢時間" },{ "QueryKey", "查詢鍵項" },
       { "IP", "IP" }
      //,{ "PsText", "備註" }
   };

   public RecordsController(IWebHostEnvironment environment, IOptions<AppSettings> appSettings, 
      IFetchesSystemService systemService, IFetchesRecordService service, IMapper mapper)
   {
      _environment = environment;
      _appSettings = appSettings.Value;
      _systemService = systemService;
      _service = service;
      _mapper = mapper;
   }
   private List<string> Keys => _KeysTitles.Keys.ToList();
   private List<string> Titles => _KeysTitles.Values.ToList();

   [HttpGet("init")]
   public async Task<ActionResult<FetchRecordsIndexModel>> Init()
   {
      var systems = await _systemService.FetchAllAsync();
      int system = 0;
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
      
      var request = new FetchRecordsFetchRequest(system, year, month);

      var departments = new List<FetchSystemDepartmentView>();
      departments.Add(new FetchSystemDepartmentView(DepartmentKeys.JUD, DepartmentTitles.JUD));
      departments.Add(new FetchSystemDepartmentView(DepartmentKeys.MOJ, DepartmentTitles.MOJ));


      return new FetchRecordsIndexModel(departments, systems.ToList(), years, request);
   }

   [HttpGet]
   public async Task<ActionResult> Index(int system, int year, int month)
   {
      FetchesSystem? selectedSystem = null;
      if (system > 0)
      {
         selectedSystem = await _systemService.GetByIdAsync(system);
         if (selectedSystem == null)
         {
            ModelState.AddModelError("system", $"查無此系統. id: {system}");
            return BadRequest(ModelState);
         }
      }
      
      if (selectedSystem != null)
      {
         var records = await _service.FetchAsync(selectedSystem, year, month);
         return Ok(records.MapViewModelList(_mapper));
      }
     
      var model = await GetSummaries(year, month);
      return Ok(model);

   }

   async Task<FetchRecordsSummaryIndex> GetSummaries(int year, int month, bool addRecords = false)
   {
      var allRecords = await _service.FetchAllAsync(year, month);
      var model = new FetchRecordsSummaryIndex();
      if (allRecords.Count() == 0) return model;

      var systems = await _systemService.FetchAllAsync();
      systems = systems.GetOrdered();
      foreach (var sys in systems)
      {
         var records = allRecords.Where(x => x.SystemId == sys.Id);
         if (records.HasItems())
         {
            string departmentTitle = (sys.Department == DepartmentKeys.JUD ? DepartmentTitles.JUD : DepartmentTitles.MOJ);
            var summary = new FetchRecordsSummary($"{departmentTitle} - {sys.Title}", records.Count());
            if (addRecords) summary.Records = records.MapViewModelList(_mapper);
            model.Summaries.Add(summary);
         }
      }
      return model;
   }

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] FetchRecordsAddRequest request)
   {
      var selectedSystem = await _systemService.GetByIdAsync(request.System);
      if (selectedSystem == null)
      {
         ModelState.AddModelError("system", $"查無此系統. id: {request.System}");
         return BadRequest(ModelState);
      }
      

      int year = request.Year;
      int month = request.Month;
      var list = await _service.FetchAsync(selectedSystem, year, month);
      if (list.HasItems())
      {
         ModelState.AddModelError("month", "此年度月份已經有資料");
         return BadRequest(ModelState);
      }

      foreach (var item in request.Records)
      {
         item.SystemId = selectedSystem.Id;
      }

      var records = request.Records.MapEntityList(_mapper);
      
       
      await _service.AddRangeAsync(records);
      return Ok();

   }


   [HttpPost("upload")]
   public async Task<ActionResult<ICollection<FetchesRecordView>>> Upload([FromForm] FetchRecordsUploadRequest request)
   {
      var file = request.File;
      var errors = ValidateFile(file!);
      AddErrors(errors);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var records = new List<FetchesRecordView>();
      using (var stream = new MemoryStream())
      {
         await file!.CopyToAsync(stream);
         using (var package = new ExcelPackage(stream))
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
               string name = worksheet.Cells[row, 1].Text;
               if (string.IsNullOrEmpty(name)) continue;

               string identifier = worksheet.Cells[row, 2].Text;
               string courtType = worksheet.Cells[row, 3].Text;
               string caseNumber = worksheet.Cells[row, 4].Text;
               string queryTime = worksheet.Cells[row, 5].Text;
               string queryKey = worksheet.Cells[row, 6].Text;
               string ip = worksheet.Cells[row, 7].Text;
               string ps = worksheet.Cells[row, 8].Text;

               var record = new FetchesRecordView()
               {
                  Name = name,
                  Identifier = identifier,
                  CaseNumber = caseNumber,
                  CourtType = courtType,
                  QueryTime = queryTime,
                  QueryKey = queryKey,
                  IP = ip,
                  Ps = ps
               };
               records.Add(record);
            }
         }
      }
      return records;
   }
   [HttpPost("reports")]
   public async Task<IActionResult> Reports(FetchRecordsFetchRequest request)
   {
      var model = await GetSummaries(request.Year, request.Month);
     
      string title = $"{request.Year}年{request.Month}月 對外連線查詢紀錄統計";
      var reportModel = new FetchRecordsReportModel(title, model.Summaries.ToList());
      var doc = new FetchesRecordReportDocument(reportModel);

      byte[] bytes = doc.GeneratePdf();
      return Ok(new BaseFileView(title, bytes));
   }
   [HttpPost("template/{key}")]
   public async Task<IActionResult> Template(string key)
   {
      string folder = "fetches";
      string filename = $"{key}.xlsx";
      string path = Path.Combine(TemplatePath(_environment, _appSettings), folder, filename);
      // Check if the file exists
      if (!System.IO.File.Exists(path))
      {
         return NotFound($"The template file '{key}.xlsx' was not found.");
      }

      // Open the file as a stream
      var memory = new MemoryStream();
      using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
      {
         await stream.CopyToAsync(memory);
      }
      memory.Position = 0;
    
      string contentType = FileContentType.Excel;
      return File(memory, contentType, filename);
   }

   [HttpPost("download")]
   public async Task<IActionResult> Download([FromBody] FetchRecordsDownloadRequest request)
   {
      bool addRecords = true;
      var model = await GetSummaries(request.Year, request.Month, addRecords);

      // Step 1: Create the Excel package
      using (var package = new ExcelPackage())
      {
         // Step 2: Add a worksheet
         var worksheet = package.Workbook.Worksheets.Add("Sheet1");
         int cols = Keys.Count;
         for (int i = 0; i < cols; i++)
         {
            int width = 15;
            if (Keys[i] == "CaseNumber") width = 30;
            else if (Keys[i] == "QueryTime") width = 30;
            else if (Keys[i] == "QueryKey") width = 35;
            worksheet.Column(i + 1).Width = width;
         }
         int rowIndex = 1;
         foreach (var summary in model.Summaries)
         {
            string title = $"{summary.System} ({summary.Records.Count}筆)";
            rowIndex = AddSystemTitle(worksheet, rowIndex, title);
            rowIndex = AddTableTitle(worksheet, rowIndex);
            foreach (var record in summary.Records)
            {
               rowIndex = AddRecord(worksheet, rowIndex, record);
            }
         }

         using (var range = worksheet.Cells[worksheet.Dimension.Address])
         {
            // Set borders
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Set alignment (center text)
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
         }

         // Step 3: Save the Excel package to a memory stream
         var stream = new MemoryStream();
         package.SaveAs(stream);
         stream.Position = 0;  // Reset stream position to the beginning

         // Step 4: Return the file as a downloadable response
         string excelFileName = "Sample.xlsx";
         string contentType = FileContentType.Excel;
         return File(stream, contentType, excelFileName);
      }
   }

   int AddSystemTitle(ExcelWorksheet worksheet, int rowIndex, string title)
   {
      worksheet.Cells[rowIndex, 1].Value = title;
      worksheet.Cells[rowIndex, 1, rowIndex, Keys.Count].Merge = true;
      worksheet.Cells[rowIndex, 1].Style.Font.Size = 18;
      worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
      worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
      worksheet.Cells[rowIndex, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

      //Set row bgcolor
      //codes goes here
      // Set row background color
      worksheet.Cells[rowIndex, 1, rowIndex, Keys.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
      worksheet.Cells[rowIndex, 1, rowIndex, Keys.Count].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Use your desired color


      worksheet.Row(rowIndex).Height = 30;

      rowIndex += 1;
      return rowIndex;
   }
   int AddTableTitle(ExcelWorksheet worksheet, int rowIndex)
   {
      for (int i = 0; i < Titles.Count; i++)
      {
         worksheet.Cells[rowIndex, i + 1].Value = Titles[i];
      }
      worksheet.Row(rowIndex).Height = 30;

      rowIndex += 1;
      
      return rowIndex;
   }
   int AddRecord(ExcelWorksheet worksheet, int rowIndex, FetchesRecordView record)
   {
      var keys = _KeysTitles.Keys.ToList();
      for (int i = 0; i < keys.Count; i++)
      {
         string key = keys[i];
         worksheet.Cells[rowIndex, i + 1].Value = record[key];
      }
      worksheet.Cells[rowIndex, 1, rowIndex, Keys.Count].Style.Font.Size = 11;
      worksheet.Row(rowIndex).Height = 20;
      rowIndex += 1;
      return rowIndex;
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