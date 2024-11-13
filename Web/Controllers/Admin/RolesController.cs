using ApplicationCore.Services;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Infrastructure.Helpers;
using Web.Models;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.Views;
using ApplicationCore.Authorization;
using Infrastructure.Paging;
using ApplicationCore.Services.Auth;
using ApplicationCore;

namespace Web.Controllers.Admin;

public class RolesController : BaseAdminController
{
   private readonly IRolesService _rolesService;
   private readonly IMapper _mapper;
   public RolesController(IRolesService rolesService, IMapper mapper)
   {
      _rolesService = rolesService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<IEnumerable<RoleViewModel>>> Index()
   {
      var roles = await _rolesService.FetchAllAsync();
      return roles.MapViewModelList(_mapper);
   }

}