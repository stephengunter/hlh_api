using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class CredentialInfoMappingProfile : Profile
{
   public CredentialInfoMappingProfile()
   {
      CreateMap<CredentialInfo, CredentialInfoViewModel>();

      CreateMap<CredentialInfoViewModel, CredentialInfo>();
   }
}