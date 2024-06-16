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
using ApplicationCore.Consts;
using Infrastructure.Entities;
using System;

namespace Web.Controllers.Api;
public class EventsController : BaseApiController
{
   private readonly IEventsService _eventsService;
   private readonly ICalendarsService _calendarsService;

   private readonly IMapper _mapper;

   public EventsController(IEventsService eventsService,
      ICalendarsService calendarsService, IMapper mapper)
   {
      _eventsService = eventsService;
      _calendarsService = calendarsService;
      _mapper = mapper;
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

   [HttpGet("create")]
   public async Task<ActionResult> Create(string? calendar)
   {
      Calendar? selectedCalendar = string.IsNullOrEmpty(calendar) ? null 
                                    : await _calendarsService.FindByKeyAsync(calendar);
     
      if (selectedCalendar == null) return Ok(new EventCreateForm());
      if(selectedCalendar.Key.EqualTo(CalendarTypes.EC))
      {
         return Ok(new EcEventForm());
      }
      return Ok(new EventCreateForm());
      
   }

   [HttpPost]
   public async Task<ActionResult<EventViewModel>> Store([FromBody] EventCreateForm model)
   {
      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new Event();
      model.SetValuesTo(entity);
      entity.StartDate = entity.StartDate.ToTaipeiTime();
      entity.EndDate = entity.EndDate.ToTaipeiTime();

      bool allowNullStartDate = false;
      bool allowNullEndDate = model.AllDay;

      bool valid = entity.IsValid(allowNullStartDate, allowNullEndDate);
      if(!valid) 
      {
          ModelState.AddModelError("endDate", "日期錯誤");
         return BadRequest(ModelState);

      }

      var calendars = model.CalendarIds.Select(id => new Calendar { Id = id }).ToList();
      var locations = new List<Location>();
      entity = await _eventsService.CreateAsync(entity, calendars, locations);

      return Ok(entity.MapViewModel(_mapper));
   }




   [HttpGet("{id}")]
   public async Task<ActionResult<EventViewModel>> Details(int id)
   {
      var entity = await _eventsService.GetByIdAsync(id);
      if (entity == null) return NotFound();
      //if (!event.Active) return NotFound();

      return entity.MapViewModel(_mapper);
   }

   async Task ValidateRequestAsync(BaseEventForm model)
   {
      if(String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫標題");
      if(!model.StartDate.HasValue) ModelState.AddModelError("startDate", "必須填寫開始日期");
    

      if (!model.AllDay)
      {
         if(!model.EndDate.HasValue) ModelState.AddModelError("endDate", "必須填寫結束日期");
      }

      if(model.CalendarIds.IsNullOrEmpty()) ModelState.AddModelError("calendarIds", "必須選擇行事曆");

      var calendars = await _calendarsService.FetchAsync(model.CalendarIds);
      if (calendars.Count() != model.CalendarIds.Count)
      { 
         foreach(var id in model.CalendarIds) 
         { 
            var calendar = calendars.FirstOrDefault(c => c.Id == id);
            if (calendar == null)
            {
               ModelState.AddModelError("calendarIds", $"行事曆 id: {id}不存在");
               break;
            }
         }
         
      }

   }

}