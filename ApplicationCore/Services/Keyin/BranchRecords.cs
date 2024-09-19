using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Models.Keyin;
using ApplicationCore.Specifications;
using ApplicationCore.Specifications.Keyin;
using Infrastructure.Helpers;

namespace ApplicationCore.Services.Keyin;

public interface IBranchRecordService
{
   Task<IEnumerable<BranchRecord>> FetchAsync(int year, int month);
   Task<int?> FindMinYearAsync();
   Task<BranchRecord?> GetByIdAsync(int id);
   Task<BranchRecord> CreateAsync(BranchRecord entity);
   Task UpdateAsync(BranchRecord entity);

   Task<BranchRecord?> FindAsync(Branch branch, int year, int month);
}

public class BranchRecordService : IBranchRecordService
{
   private readonly IDefaultRepository<BranchRecord> _repository;

   public BranchRecordService(IDefaultRepository<BranchRecord> repository)
   {
      _repository = repository;
   }
   public async Task<IEnumerable<BranchRecord>> FetchAsync(int year, int month)
      => await _repository.ListAsync(new BranchRecordSpecification(year, month));

   public async Task<BranchRecord?> FindAsync(Branch branch, int year, int month)
      => await _repository.FirstOrDefaultAsync(new BranchRecordSpecification(branch, year, month));
   public async Task<int?> FindMinYearAsync()
      => await _repository.FirstOrDefaultAsync(new BranchRecordMinYearSpecification());

   public async Task<BranchRecord?> GetByIdAsync(int id)
      => await _repository.GetByIdAsync(id);

   public async Task<BranchRecord> CreateAsync(BranchRecord entity)
      => await _repository.AddAsync(entity);

   public async Task UpdateAsync(BranchRecord entity)
   => await _repository.UpdateAsync(entity);
}
