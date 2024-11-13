using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Models.Criminals;
using ApplicationCore.Specifications;
using ApplicationCore.Specifications.Criminals;
using Infrastructure.Helpers;

namespace ApplicationCore.Services.Criminals;

public interface ICriminalFetchRecordService
{
   Task AddRangeAsync(IEnumerable<CriminalFetchRecord> records);
   Task<IEnumerable<CriminalFetchRecord>> FetchAsync(int year, int month);
   Task<int?> FindMinYearAsync();
   Task<CriminalFetchRecord?> GetByIdAsync(int id);
   Task<CriminalFetchRecord> CreateAsync(CriminalFetchRecord entity);
   Task UpdateAsync(CriminalFetchRecord entity);
}

public class CriminalFetchRecordService : ICriminalFetchRecordService
{
   private readonly IDefaultRepository<CriminalFetchRecord> _repository;

   public CriminalFetchRecordService(IDefaultRepository<CriminalFetchRecord> repository)
   {
      _repository = repository;
   }
   public async Task AddRangeAsync(IEnumerable<CriminalFetchRecord> records)
      => await _repository.AddRangeAsync(records);
   public async Task<IEnumerable<CriminalFetchRecord>> FetchAsync(int year, int month)
      => await _repository.ListAsync(new CriminalFetchRecordSpecification(year, month));
   public async Task<int?> FindMinYearAsync()
      => await _repository.FirstOrDefaultAsync(new CriminalFetchRecordMinYearSpecification());

   public async Task<CriminalFetchRecord?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<CriminalFetchRecord> CreateAsync(CriminalFetchRecord entity)
      => await _repository.AddAsync(entity);

   public async Task UpdateAsync(CriminalFetchRecord entity)
   => await _repository.UpdateAsync(entity);
}
