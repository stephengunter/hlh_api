using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Models.IT;
using ApplicationCore.Models.Keyin;
using ApplicationCore.Specifications;
using ApplicationCore.Specifications.Keyin;
using Infrastructure.Helpers;

namespace ApplicationCore.Services.Keyin;

public interface IKeyinPersonService
{
   Task<IEnumerable<KeyinPerson>> FetchAsync();
   Task<IEnumerable<KeyinPerson>> FetchAllPassAsync();
   Task<KeyinPerson?> FindByNameAsync(string name);
   Task<KeyinPerson?> GetByIdAsync(int id);
   Task<KeyinPerson> CreateAsync(KeyinPerson entity);
   Task UpdateAsync(KeyinPerson entity);
   Task RemoveAsync(KeyinPerson entity);
}

public class KeyinPersonService : IKeyinPersonService
{
   private readonly IDefaultRepository<KeyinPerson> _personsRepository;

   public KeyinPersonService(IDefaultRepository<KeyinPerson> keyinPersonsRepository)
   {
      _personsRepository = keyinPersonsRepository;
   }
   public async Task<IEnumerable<KeyinPerson>> FetchAsync()
      => await _personsRepository.ListAsync(new KeyinPersonSpecification());

   public async Task<IEnumerable<KeyinPerson>> FetchAllPassAsync()
      => await _personsRepository.ListAsync(new KeyinAllPassPersonSpecification());

   public async Task<KeyinPerson?> FindByNameAsync(string name)
      => await _personsRepository.FirstOrDefaultAsync(new KeyinPersonSpecification(name));

   public async Task<KeyinPerson?> GetByIdAsync(int id)
      => await _personsRepository.GetByIdAsync(id);

   public async Task<KeyinPerson> CreateAsync(KeyinPerson entity)
      => await _personsRepository.AddAsync(entity);

   public async Task UpdateAsync(KeyinPerson entity)
      => await _personsRepository.UpdateAsync(entity);

   public async Task RemoveAsync(KeyinPerson entity)
     => await _personsRepository.DeleteAsync(entity);
}
