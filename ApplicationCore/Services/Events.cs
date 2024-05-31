using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface IEventsService
{
   Task<IEnumerable<Category>> FetchCategoriesAsync(IList<string> keys);
   Task<IEnumerable<Event>> FetchAsync(Category category);
   Task<Event?> GetByIdAsync(int id);

   Task<Event> CreateAsync(Event entity);
   Task<Event> CreateAsync(Event entity, ICollection<Location> locations);
   Task<Event> CreateAsync(Event entity, ICollection<Calendar> calendars, ICollection<Location> locations);
   Task UpdateAsync(Event entity);
}

public class EventsService : IEventsService
{
	private readonly IDefaultRepository<Event> _eventsRepository;
   private readonly IDefaultRepository<Category> _categoriesRepository;
   private readonly IDefaultRepository<CategoryPost> _categoryPostRepository;
   private readonly PostType _type;
   private readonly DefaultContext _context;
   public EventsService(IDefaultRepository<Event> eventsRepository, IDefaultRepository<Category> categoriesRepository,
      IDefaultRepository<CategoryPost> categoryPostRepository, DefaultContext context)
	{
      _eventsRepository = eventsRepository;
      _categoriesRepository = categoriesRepository;
      _categoryPostRepository = categoryPostRepository;
      _context = context;
      _type = PostType.Event;
   }

   public async Task<IEnumerable<Category>> FetchCategoriesAsync(IList<string> keys)
      => await _categoriesRepository.ListAsync(new CategoriesSpecification(_type, keys));

   async Task<IEnumerable<CategoryPost>> FetchCategoryPostAsync(Category category)
      => await _categoryPostRepository.ListAsync(new CategoryPostsSpecification(category, _type));
   public async Task<IEnumerable<Event>> FetchAsync(Category category)
   {
      var categoryPosts = await FetchCategoryPostAsync(category);
      if(categoryPosts.IsNullOrEmpty()) return new List<Event>();

      var ids = categoryPosts.Select(p => p.Id).ToList();
      return await _eventsRepository.ListAsync(new EventSpecification(ids));
   }

   public async Task<Event?> GetByIdAsync(int id)
      => await _eventsRepository.GetByIdAsync(id);

   public async Task<Event> CreateAsync(Event entity)
		=> await _eventsRepository.AddAsync(entity);

   public async Task<Event> CreateAsync(Event entity, ICollection<Location> locations)
   {
      entity = await _eventsRepository.AddAsync(entity);

      AddLocationEvents(entity, locations);

      return entity;
   }
   void AddLocationEvents(Event entity, ICollection<Location> locations)
   {
      var locationEvents = locations.Select(location => new LocationEvent { EventId = entity.Id, LocationId = location.Id });
      _context.LocationEvents.AddRange(locationEvents);
      _context.SaveChanges();
   }
   void AddEventCalendars(Event entity, ICollection<Calendar> calendars)
   {
      var eventCalendars = calendars.Select(calendar => new EventCalendar { EventId = entity.Id, CalendarId = calendar.Id });
      _context.EventCalendars.AddRange(eventCalendars);
      _context.SaveChanges();
   }
   public async Task<Event> CreateAsync(Event entity, ICollection<Calendar> calendars, ICollection<Location> locations)
   {
      entity = await _eventsRepository.AddAsync(entity);

      AddEventCalendars(entity, calendars);
      AddLocationEvents(entity, locations);
      return entity;
   }

   public async Task UpdateAsync(Event Event)
		=> await _eventsRepository.UpdateAsync(Event);

}
