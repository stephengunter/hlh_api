using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class LocationMappingProfile : Profile
{
	public LocationMappingProfile()
	{
		CreateMap<Location, LocationViewModel>();

		CreateMap<LocationViewModel, Location>()
         .ForMember(x => x.Parent, opt => opt.Ignore());
   }
}
