using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Models.Keyin;
using ApplicationCore.Specifications;
using ApplicationCore.Specifications.Keyin;
using Infrastructure.Helpers;

namespace ApplicationCore.Services.Keyin;

public interface IPersonRecordService
{
   Task<IEnumerable<PersonRecord>> FetchAsync(int year, int month);
   Task<PersonRecord?> GetByIdAsync(int id);
   Task<PersonRecord> CreateAsync(PersonRecord entity);
   Task UpdateAsync(PersonRecord entity);
   Task<PersonRecord?> FindAsync(KeyinPerson person, int year, int month);

   Task<int?> FindMinYearAsync();
}

public class PersonRecordService : IPersonRecordService
{
   private readonly IDefaultRepository<PersonRecord> _repository;

   public PersonRecordService(IDefaultRepository<PersonRecord> repository)
   {
      _repository = repository;
   }
   public async Task<IEnumerable<PersonRecord>> FetchAsync(int year, int month)
      => await _repository.ListAsync(new PersonRecordSpecification(year, month));

   public async Task<PersonRecord?> FindAsync(KeyinPerson person, int year, int month)
      => await _repository.FirstOrDefaultAsync(new PersonRecordSpecification(person, year, month));

   public async Task<PersonRecord?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<PersonRecord> CreateAsync(PersonRecord entity)
      => await _repository.AddAsync(entity);

   public async Task UpdateAsync(PersonRecord entity)
   => await _repository.UpdateAsync(entity);

   public async Task<int?> FindMinYearAsync()
      => await _repository.FirstOrDefaultAsync(new PersonRecordMinYearSpecification());
}
