using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class DatabaseMappingProfile : Profile
{
	public DatabaseMappingProfile()
	{
		CreateMap<Database, DatabaseViewModel>();

		CreateMap<DatabaseViewModel, Database>();
	}
}
