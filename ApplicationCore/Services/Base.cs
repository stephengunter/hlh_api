using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Interfaces;
using Infrastructure.Helpers;
using System;

namespace ApplicationCore.Services;

public interface IBaseService
{
   Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(IAggregateRoot entity);
   Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(string type, string id, ICollection<string>? actions = null);
   Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(string type, ICollection<string> ids, ICollection<string> actions);
   Task<ModifyRecord?> GetModifyRecordByIdAsync(int id);

   Task<ModifyRecord> CreateModifyRecordAsync(ModifyRecord record);
   Task<IEnumerable<ModifyRecord>> CreateModifyRecordListAsync(IEnumerable<ModifyRecord> records);
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

   public async Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(string type, string id, ICollection<string>? actions = null)
   { 
      if(actions!.IsNullOrEmpty()) return await _repository.ListAsync(new ModifyRecordSpecification(type, id));
      return await _repository.ListAsync(new ModifyRecordSpecification(type, id, actions!));
   }
   public async Task<IEnumerable<ModifyRecord>> FetchModifyRecordsAsync(string type, ICollection<string> ids, ICollection<string> actions)
      => await _repository.ListAsync(new ModifyRecordSpecification(type, ids, actions));
   public async Task<ModifyRecord?> GetModifyRecordByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<ModifyRecord> CreateModifyRecordAsync(ModifyRecord record)
      => await _repository.AddAsync(record);

   public async Task<IEnumerable<ModifyRecord>> CreateModifyRecordListAsync(IEnumerable<ModifyRecord> records)
      => await _repository.AddRangeAsync(records);

   public async Task UpdateModifyRecordAsync(ModifyRecord record)
   => await _repository.UpdateAsync(record);

}
