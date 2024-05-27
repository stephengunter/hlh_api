using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Helpers;
using ApplicationCore.DataAccess;
using Newtonsoft.Json;
using ApplicationCore.Views.Jud;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings.Files;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly EventSettings _eventSettings;

   public ATestsController(IOptions<EventSettings> eventSettings)
   {
      _eventSettings = eventSettings.Value;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      

      return Ok(_eventSettings);
   }


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
