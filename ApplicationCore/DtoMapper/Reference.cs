using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ReferenceMappingProfile : Profile
{
	public ReferenceMappingProfile()
	{
		CreateMap<Reference, ReferenceViewModel>();

		CreateMap<ReferenceViewModel, Reference>();
	}
}
