using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ServerMappingProfile : Profile
{
   public ServerMappingProfile()
   {
      CreateMap<Server, ServerViewModel>();

      CreateMap<ServerViewModel, Server>();
   }
}