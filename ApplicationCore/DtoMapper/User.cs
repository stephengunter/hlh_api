using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class UserMappingProfile : AutoMapper.Profile
{
	public UserMappingProfile()
	{
		CreateMap<User, UserViewModel>();

		CreateMap<UserViewModel, User>();
	}
}

public static class UserDtoHelpers
{
	public static UserViewModel MapViewModel(this User user, IMapper mapper)
		=> mapper.Map<UserViewModel>(user);
	public static UserViewModel MapViewModel(this User user, IList<string> roles, IMapper mapper)
	{
		var model = MapViewModel(user, mapper);
		model.Roles = roles.JoinToString();
		return model;
	}
}
