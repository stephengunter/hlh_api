using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Models.Files;
using ApplicationCore.Specifications;
using Infrastructure.Consts;
using Infrastructure.Helpers;
using System;

namespace ApplicationCore.Services.Files;

public interface IJudgebookFilesService
{
   Task<IEnumerable<JudgebookType>> FetchTypesAsync();
   Task<JudgebookType?> GetTypeByIdAsync(int id);

   Task<IEnumerable<JudgebookFile>> FetchAllAsync();

   Task<IEnumerable<JudgebookFile>> FetchAsync(JudgebookType type, string include = "");
   Task<JudgebookFile?> GetByIdAsync(int id, string include = "");

   Task<JudgebookFile?> CreateAsync(JudgebookFile judgebook);
   Task UpdateAsync(JudgebookFile judgebook);
}

public class JudgebooksService : BaseService, IJudgebookFilesService, IBaseService
{
   private readonly IDefaultRepository<JudgebookFile> _repository;
   private readonly IDefaultRepository<JudgebookType> _typeRepository;

   public JudgebooksService(IDefaultRepository<ModifyRecord> modifyRecordrepository, IDefaultRepository<JudgebookFile> repository, IDefaultRepository<JudgebookType> typeRepository)
      :base(modifyRecordrepository) 
   {
      _typeRepository = typeRepository;
      _repository = repository;
   }
   public async Task<IEnumerable<JudgebookType>> FetchTypesAsync()
      => await _typeRepository.ListAsync(new JudgebookTypesSpecification());

   public async Task<JudgebookType?> GetTypeByIdAsync(int id)
      => await _typeRepository.GetByIdAsync(id);
   public async Task<IEnumerable<JudgebookFile>> FetchAllAsync()
      => await _repository.ListAsync(new JudgebookFilesSpecification());

   public async Task<IEnumerable<JudgebookFile>> FetchAsync(JudgebookType type, string include = "")
      => await _repository.ListAsync(new JudgebookFilesSpecification(type, include));

   public async Task<JudgebookFile?> GetByIdAsync(int id, string include = "")
      => await _repository.FirstOrDefaultAsync(new JudgebookFilesSpecification(id, include));

   public async Task<JudgebookFile?> CreateAsync(JudgebookFile entity)
   {
      entity = await _repository.AddAsync(entity);
      if (entity != null)
      {
         await CreateModifyRecordAsync(new ModifyRecord(entity, ActionsTypes.Create, entity.UpdatedBy!, entity.CreatedAt));
      }
      return entity;
   }

   public async Task UpdateAsync(JudgebookFile entity)
   {
      await _repository.UpdateAsync(entity);
      await CreateModifyRecordAsync(new ModifyRecord(entity, ActionsTypes.Update, entity.UpdatedBy!, entity.LastUpdated!.Value));
   }

}
