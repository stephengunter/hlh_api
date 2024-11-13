using ApplicationCore.Models.Fetches;
using ApplicationCore.Views.Fetches;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class FetchesSystemMappingProfile : Profile
{
	public FetchesSystemMappingProfile()
   {
		CreateMap<FetchesSystem, FetchesSystemView>();

		CreateMap<FetchesSystemView, FetchesSystem>();
	}
}
