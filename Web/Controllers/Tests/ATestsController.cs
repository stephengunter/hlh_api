using ApplicationCore.Models;
using ApplicationCore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Infrastructure.Helpers;
namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly ILocationsService _locationsService;
   private readonly IEventsService _eventsService;
   private readonly ICalendarsService _calendarsService;
   private readonly DefaultContext _context;
   private readonly IMapper _mapper;
   public ATestsController(IEventsService eventsService, ICalendarsService calendarsService, 
   ILocationsService locationsService, DefaultContext context, IMapper mapper)
   {
      _eventsService = eventsService;
      _calendarsService = calendarsService;
      _locationsService = locationsService;
      _context = context;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      await CreateEvents();
      
      return Ok();
   }
   [HttpGet("Categories")]
   public async Task<ActionResult> Categories()
   {
      var categories = await _eventsService.FetchCategoriesAsync();
      var list = categories.Where(c => c.Parent != null);

      return Ok(categories.MapViewModelList(_mapper));
   }
   [HttpGet("LocationEvents")]
   public ActionResult LocationEvents()
   {
      return Ok(_context.LocationEvents.ToList());
   }
   [HttpGet("EventCalendars")]
   public ActionResult EventCalendars()
   {
      return Ok(_context.EventCalendars.ToList());
   }

   async Task CreateEvents()
   {
      var categories = (await _eventsService.FetchCategoriesAsync()).ToList();
      var calendars = (await _calendarsService.FetchAsync()).ToList();
      var locations = (await _locationsService.FetchAllAsync()).ToList();
      var days = Enumerable.Range(0, 30).ToList();
      var hours = Enumerable.Range(0, 6).ToList();
      var minutes = new List<int>{0, 15, 30, 45, 60};
      var events = new List<Event>();
      var start = new DateTime(2024, 5, 1, 9, 0, 0);
      for(int i = 0; i < 50; i++)
      {
         var begin = start.AddDays(days.GetRandomItem()).AddHours(hours.GetRandomItem());
         var end = begin.AddMinutes(minutes.GetRandomItem());
         events.Add(new Event{ Title = $"test event {i + 1}", Content = $"test event content {i + 1}", 
            StartDate = begin,  EndDate = end,
            UserId = "f2003188-0fd4-4c49-aad1-3f7f9bd6338c"
         });
      }
      
      foreach(var entity in events)
      {
         await _eventsService.CreateAsync(entity, new List<Category>{ categories.GetRandomItem() },
            new List<Calendar>{ calendars.GetRandomItem() }, new List<Location>{ locations.GetRandomItem() }
         );
      }
   }

   [HttpGet("calendars")]
   public async Task<ActionResult> Calendars()
   {

      var list = await _calendarsService.FetchAsync();
      return Ok(list.MapViewModelList(_mapper));
   }
   [HttpGet("locations")]
   public async Task<ActionResult> Locations()
   {

      var list = await _locationsService.FetchAllAsync();
      return Ok(list.MapViewModelList(_mapper));
   }
   [HttpGet("events")]
   public async Task<ActionResult> Events()
   {

      var list = await _eventsService.FetchAllAsync();
      return Ok(list.MapViewModelList(_mapper));
   }



   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
