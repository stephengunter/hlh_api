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
   Task<IEnumerable<Department>> FetchAsync();
   Task<Department?> GetByIdAsync(int id);

   Task<Department> CreateAsync(Department Department);
	Task UpdateAsync(Department Department);
}

public class DepartmentsService : IDepartmentsService
{
	private readonly IDefaultRepository<Department> _departmentsRepository;

	public DepartmentsService(IDefaultRepository<Department> departmentsRepository)
	{
      _departmentsRepository = departmentsRepository;
	}
   public async Task<IEnumerable<Department>> FetchAsync()
      => await _departmentsRepository.ListAsync(new DepartmentSpecification());

   public async Task<Department?> GetByIdAsync(int id)
      => await _departmentsRepository.GetByIdAsync(id);

   public async Task<Department> CreateAsync(Department Department)
		=> await _departmentsRepository.AddAsync(Department);

		public async Task UpdateAsync(Department Department)
		=> await _departmentsRepository.UpdateAsync(Department);

}
