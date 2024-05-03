using ApplicationCore.Models.Files;
using ApplicationCore.Views.Files;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class JudgebookTypeMappingProfile : Profile
{
	public JudgebookTypeMappingProfile()
	{
		CreateMap<JudgebookType, JudgebookTypeViewModel>();

		CreateMap<JudgebookTypeViewModel, JudgebookType>();
	}
}
