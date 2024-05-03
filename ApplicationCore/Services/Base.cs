using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Models.Files;
using ApplicationCore.Specifications;
using Infrastructure.Interfaces;

namespace ApplicationCore.Services;

public interface IBaseService
{
   Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(IAggregateRoot entity);
   Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(string type, string id);
   Task<ModifyRecord?> GetModifyRecordByIdAsync(int id);

   Task<ModifyRecord> CreateModifyRecordAsync(ModifyRecord record);
   Task UpdateModifyRecordAsync(ModifyRecord record);
}
public abstract class BaseService : IBaseService
{
   private readonly IDefaultRepository<ModifyRecord> _repository;

   public BaseService(IDefaultRepository<ModifyRecord> repository)
   {
      _repository = repository;
   }
   public async Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(IAggregateRoot entity)
      => await _repository.ListAsync(new ModifyRecordSpecification(entity, entity.GetId().ToString()!));

   public async Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(string type, string id)
      => await _repository.ListAsync(new ModifyRecordSpecification(type, id));

   public async Task<ModifyRecord?> GetModifyRecordByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<ModifyRecord> CreateModifyRecordAsync(ModifyRecord record)
      => await _repository.AddAsync(record);

   public async Task UpdateModifyRecordAsync(ModifyRecord record)
   => await _repository.UpdateAsync(record);

}
