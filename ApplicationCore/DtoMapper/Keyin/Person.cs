using ApplicationCore.Models.Keyin;
using ApplicationCore.Views.Keyin;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class KeyinPersonMappingProfile : Profile
{
	public KeyinPersonMappingProfile()
	{
		CreateMap<KeyinPerson, KeyinPersonView>();

		CreateMap<KeyinPersonView, KeyinPerson>();
	}
}
