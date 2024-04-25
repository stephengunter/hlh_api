using ApplicationCore.DataAccess;
using ApplicationCore.Models.Files;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services.Files;

public interface IJudgebookFilesService
{
   Task<IEnumerable<JudgebookFile>> FetchAllAsync();

   Task<JudgebookFile?> FindAsync(JudgebookFile model);
   Task<JudgebookFile?> GetByIdAsync(int id);

   Task<JudgebookFile> CreateAsync(JudgebookFile judgebook);
   Task UpdateAsync(JudgebookFile judgebook);
}

public class JudgebooksService : IJudgebookFilesService
{
   private readonly IDefaultRepository<JudgebookFile> _repository;

   public JudgebooksService(IDefaultRepository<JudgebookFile> repository)
   {
      _repository = repository;
   }
   public async Task<IEnumerable<JudgebookFile>> FetchAllAsync()
      => await _repository.ListAsync(new JudgebookFilesSpecification());

   public async Task<JudgebookFile?> FindAsync(JudgebookFile model)
      => await _repository.FirstOrDefaultAsync(new JudgebookFilesSpecification(model));

   public async Task<JudgebookFile?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<JudgebookFile> CreateAsync(JudgebookFile entity)
      => await _repository.AddAsync(entity);

   public async Task UpdateAsync(JudgebookFile entity)
   => await _repository.UpdateAsync(entity);

}
