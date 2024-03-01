using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface ILocationsService
{
   Task<IEnumerable<Location>> FetchAsync(Location? parent);
   Task<IEnumerable<Location>> FetchAsync(IEnumerable<int> ids);
   Task<IEnumerable<Location>> FetchAllAsync();
   Task<Location?> GetByIdAsync(int id);
   Task<Location?> FindByKeyAsync(string key);

   Task<Location> CreateAsync(Location location);
	Task UpdateAsync(Location location);
   Task UpdateRangeAsync(IEnumerable<Location> locations);
}

public class LocationsService : ILocationsService
{
	private readonly IDefaultRepository<Location> _locationsRepository;

	public LocationsService(IDefaultRepository<Location> locationsRepository)
	{
      _locationsRepository = locationsRepository;
	}
   public async Task<IEnumerable<Location>> FetchAsync(Location? parent)
      => await _locationsRepository.ListAsync(new LocationSpecification(parent));
   public async Task<IEnumerable<Location>> FetchAsync(IEnumerable<int> ids)
      => await _locationsRepository.ListAsync(new LocationSpecification(ids));

   public async Task<IEnumerable<Location>> FetchAllAsync()
      => await _locationsRepository.ListAsync(new LocationSpecification());

   public async Task<Location?> GetByIdAsync(int id)
      => await _locationsRepository.GetByIdAsync(id);

   public async Task<Location?> FindByKeyAsync(string key)
      => await _locationsRepository.FirstOrDefaultAsync(new LocationSpecification(key));

   public async Task<Location> CreateAsync(Location location)
		=> await _locationsRepository.AddAsync(location);

	public async Task UpdateAsync(Location location)
	=> await _locationsRepository.UpdateAsync(location);

   public async Task UpdateRangeAsync(IEnumerable<Location> locations)
   => await _locationsRepository.UpdateRangeAsync(locations);

}
