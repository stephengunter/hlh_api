using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{

   public ATestsController()
   {
      
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      

      return Ok();
   }


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
