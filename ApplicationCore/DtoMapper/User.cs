using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class UserMappingProfile : Profile
{
	public UserMappingProfile()
	{
		CreateMap<User, UserViewModel>();

		CreateMap<UserViewModel, User>();
	}
}
public class RoleMappingProfile : Profile
{
   public RoleMappingProfile()
   {
      CreateMap<Role, RoleViewModel>();

      CreateMap<RoleViewModel, Role>();
   }
}

