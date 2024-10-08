using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ApplicationCore.Consts;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface IDepartmentsService
{
   Task<IEnumerable<Department>> FetchAsync(Department? parent);
   Task<IEnumerable<Department>> FetchAsync(IEnumerable<int> ids);
   Task<IEnumerable<Department>> FetchAllAsync();
   Task<Department?> GetByIdAsync(int id);
   Task<Department?> FindByKeyAsync(string key);
   Task<Department?> FindByTitleAsync(string title);

   Task<Department> CreateAsync(Department department);
	Task UpdateAsync(Department department);
   Task UpdateRangeAsync(IEnumerable<Department> departments);
}

public class DepartmentsService : IDepartmentsService
{
	private readonly IDefaultRepository<Department> _departmentsRepository;

	public DepartmentsService(IDefaultRepository<Department> departmentsRepository)
	{
      _departmentsRepository = departmentsRepository;
	}
   public async Task<IEnumerable<Department>> FetchAsync(Department? parent)
      => await _departmentsRepository.ListAsync(new DepartmentSpecification(parent));
   public async Task<IEnumerable<Department>> FetchAsync(IEnumerable<int> ids)
      => await _departmentsRepository.ListAsync(new DepartmentSpecification(ids));

   public async Task<IEnumerable<Department>> FetchAllAsync()
      => await _departmentsRepository.ListAsync(new DepartmentSpecification());

   public async Task<Department?> GetByIdAsync(int id)
      => await _departmentsRepository.GetByIdAsync(id);

   public async Task<Department?> FindByKeyAsync(string key)
      => await _departmentsRepository.FirstOrDefaultAsync(new DepartmentSpecification(key));

   public async Task<Department?> FindByTitleAsync(string title)
      => await _departmentsRepository.FirstOrDefaultAsync(new DepartmentTitleSpecification(title));

   public async Task<Department> CreateAsync(Department department)
		=> await _departmentsRepository.AddAsync(department);

	public async Task UpdateAsync(Department department)
	=> await _departmentsRepository.UpdateAsync(department);

   public async Task UpdateRangeAsync(IEnumerable<Department> departments)
   => await _departmentsRepository.UpdateRangeAsync(departments);

}
