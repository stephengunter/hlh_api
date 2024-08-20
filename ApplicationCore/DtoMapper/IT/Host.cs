using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class HostMappingProfile : Profile
{
   public HostMappingProfile()
   {
      CreateMap<Host, HostViewModel>();

      CreateMap<HostViewModel, Host>();
   }
}