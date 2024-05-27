using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface IEventsService
{
   Task<IEnumerable<Event>> FetchAsync();
   Task<Event?> GetByIdAsync(int id);

   Task<Event> CreateAsync(Event Event);
	Task UpdateAsync(Event Event);
}

public class EventsService : IEventsService
{
	private readonly IDefaultRepository<Event> _eventsRepository;

   public EventsService(IDefaultRepository<Event> eventsRepository)
	{
      _eventsRepository = eventsRepository;
   }
   public async Task<IEnumerable<Event>> FetchAsync()
      => await _eventsRepository.ListAsync(new EventSpecification());

   public async Task<Event?> GetByIdAsync(int id)
      => await _eventsRepository.GetByIdAsync(id);

   public async Task<Event> CreateAsync(Event Event)
		=> await _eventsRepository.AddAsync(Event);

		public async Task UpdateAsync(Event Event)
		=> await _eventsRepository.UpdateAsync(Event);

}
