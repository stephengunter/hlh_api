using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface ICalendarsService
{
   Task<IEnumerable<Calendar>> FetchAsync();
   Task<IEnumerable<Calendar>> FetchAsync(ICollection<string> keys);
   Task<IEnumerable<Calendar>> FetchAsync(ICollection<int> ids);
   Task<Calendar?> FindByKeyAsync(string key);
   Task<Calendar?> GetByIdAsync(int id);

   Task<Calendar> CreateAsync(Calendar entity);
   Task UpdateAsync(Calendar entity);

   Task<IEnumerable<Calendar>> FetchByEventAsync(Event eventEntity);
}

public class CalendarsService : ICalendarsService
{
   private readonly IDefaultRepository<Calendar> _calendarsRepository;
   private readonly PostType _type;
   private readonly DefaultContext _context;
   public CalendarsService(IDefaultRepository<Calendar> calendarsRepository, DefaultContext context)      
	{
      _calendarsRepository = calendarsRepository;
      _context = context;
   }
   public async Task<IEnumerable<Calendar>> FetchAsync()
      => await _calendarsRepository.ListAsync(new CalendarsSpecification());
   public async Task<IEnumerable<Calendar>> FetchAsync(ICollection<string> keys)
      => await _calendarsRepository.ListAsync(new CalendarsSpecification(keys));
   public async Task<IEnumerable<Calendar>> FetchAsync(ICollection<int> ids)
     => await _calendarsRepository.ListAsync(new CalendarsSpecification(ids));
   public async Task<Calendar?> GetByIdAsync(int id)
      => await _calendarsRepository.GetByIdAsync(id);
   public async  Task<Calendar?> FindByKeyAsync(string key)
      => await _calendarsRepository.FirstOrDefaultAsync(new CalendarsSpecification(key));

   public async Task<Calendar> CreateAsync(Calendar entity)
		=> await _calendarsRepository.AddAsync(entity);
   public async Task UpdateAsync(Calendar entity)
		=> await _calendarsRepository.UpdateAsync(entity);

   public async Task<IEnumerable<Calendar>> FetchByEventAsync(Event eventEntity)
   {
      var calendarIds = _context.EventCalendars.Where(ec => ec.EventId == eventEntity.Id)
         .Select(ec => ec.CalendarId).ToList();
      return await FetchAsync(calendarIds);
   }

}
