using ApplicationCore.Services;
using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.DtoMapper;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Web.Controllers;

public class UsersController : BaseAdminController
{
   private readonly IUsersService _usersService;
   private readonly IDepartmentsService _departmentsService;
   private readonly IMapper _mapper;

  
   public UsersController(IUsersService usersService, IDepartmentsService departmentsService, IMapper mapper)
   {
      _usersService = usersService;
      _departmentsService = departmentsService;
      _mapper = mapper;
   }
   [HttpGet("")]
   public async Task<ActionResult<UsersAdminModel>> Index(bool active, string? role, string? keyword, int page = 1, int pageSize = 10)
   {
      var request = new UsersAdminRequest(active, role, keyword, page, pageSize);
      var model = new UsersAdminModel(request);

      var selectedRole = await GetRoleAsync(request);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      if (request.Page < 1) //初次載入頁面
      {
         var roles = _usersService.FetchRoles();
         model.LoadRolesOptions(roles);

         request.Page = 1;
      }

      var users = await _usersService.FetchAsync(selectedRole);

      users = users.Where(u => u.Active == request.Active);

      var keywords = request.Keyword.GetKeywords();
      if (keywords.HasItems()) users = users.FilterByKeyword(keywords);

      var pagedList = users.GetPagedList(_mapper, page, pageSize);

      model.PagedList = users.GetPagedList(_mapper, page, pageSize);

      return model;
   }

   async Task<IdentityRole?> GetRoleAsync(UsersAdminRequest request)
   {
      if (String.IsNullOrEmpty(request.Role)) return null;
      var role = await _usersService.FindRoleAsync(request.Role);
      if (role is null)
      {
         ModelState.AddModelError("role", $"Role '{ request.Role }' not found.");
         return null;
      }
      return role;
   }

}