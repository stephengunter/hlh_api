using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class SystemAppMappingProfile : Profile
{
   public SystemAppMappingProfile()
   {
      CreateMap<SystemApp, SystemAppViewModel>();

      CreateMap<SystemAppViewModel, SystemApp>();
   }
}