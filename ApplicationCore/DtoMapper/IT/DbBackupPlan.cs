using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class DbBackupPlanMappingProfile : Profile
{
   public DbBackupPlanMappingProfile()
   {
      CreateMap<DbBackupPlan, DbBackupPlanViewModel>();

      CreateMap<DbBackupPlanViewModel, DbBackupPlan>();
   }
}