
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
using static Web.Models.EventsIndexModel;
using Infrastructure.Consts;
using Web.Models.Files;

namespace Web.Controllers.Api
{
   public class EventsController : BaseApiController
   {
      private readonly EventSettings _eventSettings;
      private readonly IEventsService _eventsService;
      
      private readonly IMapper _mapper;

      public EventsController(IOptions<EventSettings> eventSettings, IEventsService eventsService, 
         IMapper mapper)
      {
         _eventSettings = eventSettings.Value;
         _eventsService = eventsService;
         _mapper = mapper;
      }
      async Task<ICollection<Category>> GetEventCategoriesAsync()
      {
         var keys = _eventSettings.Categories.Select(c => c.Key).ToList();
         var categories = await _eventsService.FetchCategoriesAsync(keys);
         return categories.ToList();
      }

      [HttpGet("categories")]
      public async Task<ActionResult<IEnumerable<CategoryViewModel>>> Categories()
      {
         var categories = await GetEventCategoriesAsync();

         return categories.GetOrdered().MapViewModelList(_mapper);
      }

      [HttpGet("calendar/{key}")]
      public async Task<ActionResult<CanlendarResponse>> Calendar(string key, int year, int month)
      {
         var categories = await GetEventCategoriesAsync();
         var category = categories.FirstOrDefault(x => x.Key.EqualTo(key));

         if (category == null)
         {
            ModelState.AddModelError("key", $"category not found. key = {key}");
            return BadRequest(ModelState);
         }

         var request = new CanlendarRequest(category, year, month);
         var model = new CanlendarResponse(request);

         var events = await _eventsService.FetchAsync(category);

         events = events.GetOrdered();

         model.List = events.MapViewModelList(_mapper);

         return model;
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


}