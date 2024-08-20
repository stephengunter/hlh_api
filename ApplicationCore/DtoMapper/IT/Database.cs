using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
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