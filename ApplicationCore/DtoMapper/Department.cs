using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class DepartmentMappingProfile : AutoMapper.Profile
{
	public DepartmentMappingProfile()
	{
		CreateMap<Department, DepartmentViewModel>();

		CreateMap<DepartmentViewModel, Department>();
	}
}
