using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Infrastructure.Views;
using QuestPDF.Fluent;
using ApplicationCore.Views.IT;
using Web.Models.IT;

namespace Web.Controllers.Open.IT;

[Route("open/it/[controller]")]
public class SupportsController : BaseOpenController
{
   private readonly IITServiceSupport _supportsService;
   
   public SupportsController(IITServiceSupport supportsService)
   {
      _supportsService = supportsService;
   }
   [HttpGet("init")]
   public ActionResult<SupportRecordsIndexModel> Init()
   {
      var today = DateTime.Today;
      int year = today.Year - 1911;
      int month = today.Month - 1;

      int minYear = year - 1;
      var years = new List<int>();
      for (int i = minYear; i <= year; i++)
      {
         years.Add(i);
      }

      var request = new SupportRecordsFetchRequest(year, month);

      return new SupportRecordsIndexModel(years, request);
   }
   [HttpGet]
   public ICollection<SupportViewModel> Index(int year, int month)
   {
      var records = _supportsService.Fetch(year + 1911, month);
      
      return records.ToList();
   }
   [HttpPost("reports")]
   public IActionResult Reports(SupportRecordsFetchRequest request)
   {
      var records = _supportsService.Fetch(request.Year + 1911, request.Month);
      var depts = records.Select(x => x.Department).Distinct().ToList();

      var groups = new List<ITSupportGroup>();
      foreach (var dept in depts)
      {
         var items = records.Where(x => x.Department == dept).ToList();
         groups.Add(new ITSupportGroup { Title = dept, Records = items });
      }

      string title = $"資訊室 {request.Year}年{request.Month}月資訊業務服務明細表";
      var model = new SupportRecordReportModel(title, groups);

      var doc = new SupportRecordDetailsDocument(model);
      byte[] bytes = doc.GeneratePdf();
      return Ok(new BaseFileView(title, bytes));
   }
   [HttpPost("summary_report")]
   public IActionResult SummaryReport(SupportRecordsFetchRequest request)
   {
      var records = _supportsService.Fetch(request.Year + 1911, request.Month);

      var kinds = records.Select(x => x.Kind).Distinct().ToList();
      var groups = new List<ITSupportGroup>();
      foreach (var kind in kinds)
      {
         var items = records.Where(x => x.Kind == kind).ToList();
         groups.Add(new ITSupportGroup { Title = kind, Records = items });
      }

      string title = $"資訊室 {request.Year}年{request.Month}月資訊業務服務統計表";
      var model = new SupportRecordReportModel(title, groups);

      var doc = new SupportRecordReportDocument(model);

      byte[] bytes = doc.GeneratePdf();
      return Ok(new BaseFileView(title, bytes));
   }

}