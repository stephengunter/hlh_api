using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Models.Files;
using ApplicationCore.Specifications;
using Ardalis.Specification;
using Infrastructure.Consts;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using System;

namespace ApplicationCore.Services.Files;

public interface IJudgebookFilesService
{
   Task<IEnumerable<JudgebookType>> FetchTypesAsync();
   Task<JudgebookType?> GetTypeByIdAsync(int id);

   Task<IEnumerable<JudgebookFile>> FetchAllAsync(string include = "");

   Task<IEnumerable<JudgebookFile>> FetchAsync(JudgebookType type, string include = "");
   Task<IEnumerable<JudgebookFile>> FetchAsync(IEnumerable<int> ids, string include = "");
   Task<JudgebookFile?> GetByIdAsync(int id, string include = "");

   Task<IEnumerable<JudgebookFile>> FetchSameCaseEntriesAsync(JudgebookFile model, string include = "");

   Task<JudgebookFile?> CreateAsync(JudgebookFile judgebook, string ip);
   Task UpdateAsync(JudgebookFile judgebook, string ip);

   Task ReviewRangeAsync(IEnumerable<JudgebookFile> judgebooks, string userId, string ip);
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
   public async Task<IEnumerable<JudgebookFile>> FetchAllAsync(string include = "")
      => await _repository.ListAsync(new JudgebookFilesSpecification(include));

   public async Task<IEnumerable<JudgebookFile>> FetchAsync(JudgebookType type, string include = "")
      => await _repository.ListAsync(new JudgebookFilesSpecification(type, include));

   public async Task<IEnumerable<JudgebookFile>> FetchAsync(IEnumerable<int> ids, string include = "")
      => await _repository.ListAsync(new JudgebookFilesSpecification(ids, include));

   public async Task<JudgebookFile?> GetByIdAsync(int id, string include = "")
      => await _repository.FirstOrDefaultAsync(new JudgebookFilesSpecification(id, include));

   public async Task<IEnumerable<JudgebookFile>> FetchSameCaseEntriesAsync(JudgebookFile model, string include = "")
      => await _repository.ListAsync(new JudgebookFileSameSaceSpecification(model, include));

   public async Task<JudgebookFile?> CreateAsync(JudgebookFile entity, string ip)
   {
      entity = await _repository.AddAsync(entity);
      var modifyRecord = ModifyRecord.Create(entity!, ActionsTypes.Create, entity.CreatedBy, ip);
      if (entity != null)
      {
         await CreateModifyRecordAsync(modifyRecord);
      }
      return entity;
   }

   public async Task UpdateAsync(JudgebookFile entity, string ip)
   {
      var existingEntity = await _repository.GetByIdAsync(entity.Id);
      string userId = entity.UpdatedBy!;

      var modifyRecord = ModifyRecord.Create(existingEntity!, ActionsTypes.Update, entity.UpdatedBy!, ip);

      if (entity.Reviewed) entity.ReviewedBy = userId;

      await _repository.UpdateAsync(entity);
      await CreateModifyRecordAsync(modifyRecord);

      if (entity.Reviewed) 
      {
         await CreateModifyRecordAsync(ModifyRecord.Create(entity, ActionsTypes.Review, userId, ip));
      }
   }

   public async Task ReviewRangeAsync(IEnumerable<JudgebookFile> judgebooks, string userId, string ip)
   {
      var modifyRecords = judgebooks.Select(judgebook => ModifyRecord.Create(judgebook, ActionsTypes.Review, userId, ip));
      foreach (var item in judgebooks)
      {
         item.Reviewed = true;
         item.ReviewedAt = DateTime.Now;
         item.ReviewedBy = userId;
      }
      await _repository.UpdateRangeAsync(judgebooks);
      await CreateModifyRecordListAsync(modifyRecords);
   }

}
