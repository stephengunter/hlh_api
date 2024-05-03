using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Interfaces;

namespace ApplicationCore.Services;

public interface IModifyRecordsService
{
   Task<IEnumerable<ModifyRecord>> FetchAsync(IAggregateRoot entity);
   Task<ModifyRecord?> GetByIdAsync(int id);

   Task<ModifyRecord> CreateAsync(ModifyRecord ModifyRecord);
	Task UpdateAsync(ModifyRecord ModifyRecord);
}

public class ModifyRecordsService : IModifyRecordsService
{
	private readonly IDefaultRepository<ModifyRecord> _repository;

	public ModifyRecordsService(IDefaultRepository<ModifyRecord> repository)
	{
      _repository = repository;
	}
   public async Task<IEnumerable<ModifyRecord>> FetchAsync(IAggregateRoot entity)
      => await _repository.ListAsync(new ModifyRecordSpecification(entity, entity.GetId().ToString()!));

   public async Task<ModifyRecord?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<ModifyRecord> CreateAsync(ModifyRecord ModifyRecord)
		=> await _repository.AddAsync(ModifyRecord);

		public async Task UpdateAsync(ModifyRecord ModifyRecord)
		=> await _repository.UpdateAsync(ModifyRecord);

}
