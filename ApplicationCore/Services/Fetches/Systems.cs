using ApplicationCore.DataAccess;
using ApplicationCore.Models.Fetches;
using ApplicationCore.Specifications.Fetches;

namespace ApplicationCore.Services.Fetches;

public interface IFetchesSystemService
{
   Task<IEnumerable<FetchesSystem>> FetchAllAsync();
   Task<FetchesSystem?> FindByTitleAsync(string title);
   Task<FetchesSystem?> GetByIdAsync(int id);
   Task<FetchesSystem> CreateAsync(FetchesSystem entity);
   Task UpdateAsync(FetchesSystem entity);
}

public class FetchesSystemService : IFetchesSystemService
{
   private readonly IDefaultRepository<FetchesSystem> _repository;

   public FetchesSystemService(IDefaultRepository<FetchesSystem> repository)
   {
      _repository = repository;
   }
   public async Task<IEnumerable<FetchesSystem>> FetchAllAsync()
      => await _repository.ListAsync();
   public async Task<FetchesSystem?> FindByTitleAsync(string title)
      => await _repository.FirstOrDefaultAsync(new FetchSystemSpecification(title));

   public async Task<FetchesSystem?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<FetchesSystem> CreateAsync(FetchesSystem entity)
      => await _repository.AddAsync(entity);

   public async Task UpdateAsync(FetchesSystem entity)
   => await _repository.UpdateAsync(entity);
}
