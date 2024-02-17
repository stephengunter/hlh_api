using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class DepartmentMappingProfile : Profile
{
	public DepartmentMappingProfile()
	{
		CreateMap<Department, DepartmentViewModel>();

		CreateMap<DepartmentViewModel, Department>()
         .ForMember(x => x.Parent, opt => opt.Ignore());
   }
}
