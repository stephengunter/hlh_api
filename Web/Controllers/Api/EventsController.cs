
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

namespace Web.Controllers.Api
{
   public class EventsController : BaseApiController
   {
      private readonly EventSettings _eventSettings;
      private readonly IEventsService _eventsService;
      private readonly ICategorysService _categoriesService;
      
      private readonly IMapper _mapper;

      public EventsController(IOptions<EventSettings> eventSettings, IEventsService eventsService, ICategorysService categoriesService,
         IMapper mapper)
      {
         _eventSettings = eventSettings.Value;
         _eventsService = eventsService;
         _categoriesService = categoriesService;
         _mapper = mapper;
      }

      private PostType Type => PostType.Event;
      async Task<ICollection<Category>> GetEventCategoriesAsync()
      {
         var keys = _eventSettings.Categories.Select(c => c.Key).ToList();
         var categories = await _categoriesService.FetchByKeysAsync(keys, Type);
         return categories.ToList();
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


      [HttpGet("")]
      public async Task<ActionResult<IEnumerable<EventViewModel>>> Index()
      {

         var events = await _eventsService.FetchAsync();

         //events = events.Where(x => x.Active);

         events = events.GetOrdered();

         return events.MapViewModelList(_mapper);
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