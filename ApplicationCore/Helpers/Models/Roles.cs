using ApplicationCore.Views;
using ApplicationCore.Models;
using Infrastructure.Helpers;
using AutoMapper;
using Infrastructure.Paging;
using Microsoft.Isam.Esent.Interop;

namespace ApplicationCore.Helpers;

public static class RolesHelpers
{
   public static RoleViewModel MapViewModel(this Role role, IMapper mapper)
      => mapper.Map<RoleViewModel>(role);

   public static List<RoleViewModel> MapViewModelList(this IEnumerable<Role> roles, IMapper mapper)
      => roles.Select(item => MapViewModel(item, mapper)).ToList();
}