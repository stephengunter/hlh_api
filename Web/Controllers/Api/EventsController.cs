
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
   [HttpGet("{calendar}/{year}/{month}")]
   public async Task<ActionResult> Fetch(string calendar, int year, int month)
   {
      var selectedCalendar = await _calendarsService.FindByKeyAsync(calendar);
      if (selectedCalendar == null)
      {
         ModelState.AddModelError("calendars", $"calendar key={calendar} not found.");
         return BadRequest(ModelState);
      }
      var start = DateTimeHelpers.GetFirstDayOfMonth(year, month).ToStartDate();
      var end = DateTimeHelpers.GetLastDayOfMonth(year, month).ToEndDate();
      var events = await _eventsService.FetchAsync(selectedCalendar, start, end);
      return Ok();
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