using Microsoft.AspNetCore.Mvc;
using UtfUnknown;
using ApplicationCore.Views.AD;
using OfficeOpenXml;
using System.Text;
using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views.Keyin;

namespace Web.Controllers.Open;

public class TestsController : BaseOpenController
{
	public TestsController()
	{
      
	}


   [HttpPost("reports")]
   public async Task<IActionResult> Reports(PersonRecordReportRequest request)
   {
      return Ok(new Department { Id = 3, Title = "damn" });
   }

}
