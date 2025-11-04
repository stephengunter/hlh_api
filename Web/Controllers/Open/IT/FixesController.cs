using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Infrastructure.Views;
using QuestPDF.Fluent;
using ApplicationCore.Views.IT;
using Web.Models.IT;

namespace Web.Controllers.Open.IT;

[Route("open/it/[controller]")]
public class FixesController : BaseOpenController
{
   private readonly IServiceFixes _fixesService;
   
   public FixesController(IServiceFixes fixesService)
   {
      _fixesService = fixesService;
   }
   [HttpGet("init")]
   public ActionResult<FixRecordsIndexModel> Init()
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

      var request = new FixRecordsFetchRequest(year, month);

      return new FixRecordsIndexModel(years, request);
   }
   [HttpGet]
   public ICollection<FixViewModel> Index(int year, int month)
   {
      var records = _fixesService.Fetch(year + 1911, month);
      return records.ToList();
   }
   [HttpPost("reports")]
   public IActionResult Reports(FixRecordsFetchRequest request)
   {
      var records = _fixesService.Fetch(request.Year + 1911, request.Month);

      string title = $"資訊室 {request.Year}年{request.Month}月設備(料件)換修記錄表";
      var model = new FixRecordReportModel(title, records.ToList());

      var doc = new FixRecordDetailsDocument(model);

      byte[] bytes = doc.GeneratePdf();
      return Ok(new BaseFileView(title, bytes));
   }

}