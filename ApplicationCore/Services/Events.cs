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

   Task<Event> CreateAsync(Event Event);
	Task UpdateAsync(Event Event);
}

public class EventsService : IEventsService
{
	private readonly IDefaultRepository<Event> _eventsRepository;
   private readonly IDefaultRepository<Category> _categoriesRepository;
   private readonly IDefaultRepository<CategoryPost> _categoryPostRepository;
   private readonly PostType _type;
   public EventsService(IDefaultRepository<Event> eventsRepository, IDefaultRepository<Category> categoriesRepository,
      IDefaultRepository<CategoryPost> categoryPostRepository)
	{
      _eventsRepository = eventsRepository;
      _categoriesRepository = categoriesRepository;
      _categoryPostRepository = categoryPostRepository;
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

   public async Task<Event> CreateAsync(Event Event)
		=> await _eventsRepository.AddAsync(Event);

		public async Task UpdateAsync(Event Event)
		=> await _eventsRepository.UpdateAsync(Event);

}
