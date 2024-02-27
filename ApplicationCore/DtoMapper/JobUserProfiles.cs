using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class JobUserProfilesMappingProfile : Profile
{
   public JobUserProfilesMappingProfile()
   {
      CreateMap<JobUserProfiles, JobUserProfilesViewModel>();

      CreateMap<JobUserProfilesViewModel, JobUserProfiles>();
   }
}
