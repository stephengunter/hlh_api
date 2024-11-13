using ApplicationCore.DataAccess;
using ApplicationCore.Models.Fetches;
using ApplicationCore.Specifications.Fetches;

namespace ApplicationCore.Services.Fetches;

public interface IFetchesRecordService
{
   Task AddRangeAsync(IEnumerable<FetchesRecord> records);
   Task<IEnumerable<FetchesRecord>> FetchAllAsync(int year, int month);
   Task<IEnumerable<FetchesRecord>> FetchAsync(FetchesSystem system, int year, int month);
   Task<int?> FindMinYearAsync();
   Task<FetchesRecord?> GetByIdAsync(int id);
   Task<FetchesRecord> CreateAsync(FetchesRecord entity);
   Task UpdateAsync(FetchesRecord entity);
}

public class FetchesRecordService : IFetchesRecordService
{
   private readonly IDefaultRepository<FetchesRecord> _repository;

   public FetchesRecordService(IDefaultRepository<FetchesRecord> repository)
   {
      _repository = repository;
   }
   public async Task AddRangeAsync(IEnumerable<FetchesRecord> records)
      => await _repository.AddRangeAsync(records);
   public async Task<IEnumerable<FetchesRecord>> FetchAllAsync(int year, int month)
     => await _repository.ListAsync(new FetchRecordSpecification(year, month));
   public async Task<IEnumerable<FetchesRecord>> FetchAsync(FetchesSystem system, int year, int month)
      => await _repository.ListAsync(new FetchRecordSpecification(system, year, month));
   public async Task<int?> FindMinYearAsync()
      => await _repository.FirstOrDefaultAsync(new FetchRecordMinYearSpecification());

   public async Task<FetchesRecord?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<FetchesRecord> CreateAsync(FetchesRecord entity)
      => await _repository.AddAsync(entity);

   public async Task UpdateAsync(FetchesRecord entity)
   => await _repository.UpdateAsync(entity);
}
