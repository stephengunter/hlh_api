using ApplicationCore.Models.Files;
using ApplicationCore.Views.Files;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class JudgebookFileMappingProfile : Profile
{
	public JudgebookFileMappingProfile()
	{
		CreateMap<JudgebookFile, JudgebookFileViewModel>();

		CreateMap<JudgebookFileViewModel, JudgebookFile>();
	}
}
