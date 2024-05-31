using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly IEventsService _eventsService;
   public ATestsController(IEventsService eventsService)
   {
      _eventsService = eventsService;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var start = new DateTime(2024, 5, 20, 9, 0, 0);
      var end = new DateTime(2024, 5, 20, 10, 30, 0);
      var entity = new ApplicationCore.Models.Event() { Title = "test title", StartDate = start, EndDate = end };
      entity.Content = "Test Content";
      entity.UserId = "f2003188-0fd4-4c49-aad1-3f7f9bd6338c";

      var calendar = new Calendar() { Id = 1 };
      var location = new Location() { Id = 55 };

      await _eventsService.CreateAsync(entity, new List<Calendar>() { calendar }, new List<Location>());
      return Ok();
   }



   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
