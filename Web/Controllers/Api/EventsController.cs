using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Web.Models;
using ApplicationCore.Migrations;

namespace Web.Controllers.Api;
public class EventsController : BaseApiController
{
   private readonly EventSettings _eventSettings;
   private readonly IEventsService _eventsService;
   private readonly ICalendarsService _calendarsService;

   private readonly IMapper _mapper;

   public EventsController(IOptions<EventSettings> eventSettings, IEventsService eventsService,
      ICalendarsService calendarsService, IMapper mapper)
   {
      _eventSettings = eventSettings.Value;
      _eventsService = eventsService;
      _calendarsService = calendarsService;
      _mapper = mapper;
   }
   async Task<ICollection<Category>> GetEventCategoriesAsync()
   {
      var keys = _eventSettings.Categories.Select(c => c.Key).ToList();
      var categories = await _eventsService.FetchCategoriesAsync(keys);
      return categories.ToList();
   }
   [HttpGet("{calendar}/{start}/{end}")]
   public async Task<ActionResult<IEnumerable<EventViewModel>>> Fetch(string calendar, string start, string end)
   {
      var selectedCalendar = await _calendarsService.FindByKeyAsync(calendar);
      if (selectedCalendar == null)
      {
         ModelState.AddModelError("calendars", $"calendar key={calendar} not found.");
         return BadRequest(ModelState);
      }

      var startDate = start.ToStartDate();
      if(!startDate.HasValue)
      {
         ModelState.AddModelError("start", $"param start: {start} is not valid date.");
         return BadRequest(ModelState);
      }
      var endDate = end.ToStartDate();
      if (!endDate.HasValue)
      {
         ModelState.AddModelError("end", $"param end: {end} is not valid date.");
         return BadRequest(ModelState);
      }

      var events = await _eventsService.FetchAsync(selectedCalendar, startDate.Value, endDate.Value);
      return Ok(events.MapViewModelList(_mapper));
   }

   [HttpGet("categories")]
   public async Task<ActionResult<IEnumerable<CategoryViewModel>>> Categories()
   {
      var categories = await GetEventCategoriesAsync();

      return categories.GetOrdered().MapViewModelList(_mapper);
   }



   [HttpGet("create")]
   public ActionResult<EventCreateForm> Create()
   {
      return new EventCreateForm();
   }





   [HttpGet("{id}")]
   public async Task<ActionResult<EventViewModel>> Details(int id)
   {
      var entity = await _eventsService.GetByIdAsync(id);
      if (entity == null) return NotFound();
      //if (!event.Active) return NotFound();

      return entity.MapViewModel(_mapper);
   }

}