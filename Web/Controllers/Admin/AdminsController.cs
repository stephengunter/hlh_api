using ApplicationCore.Exceptions;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using ApplicationCore.DtoMapper;
using Web.Filters;
using Web.Models;
using ApplicationCore.Consts;
using ApplicationCore.Models;
using Infrastructure.Helpers;

namespace Web.Controllers.Admin;

public class AdminsController : BaseAdminController
{
   private readonly IUsersService _usersService;
   private readonly AdminSettings _adminSettings; 
   private readonly IMapper _mapper;

  
   public AdminsController(IUsersService usersService, IOptions<AdminSettings> adminSettings,
      IMapper mapper)
   {
      _usersService = usersService;
      _adminSettings = adminSettings.Value;
      _mapper = mapper;
   }
   [HttpGet]
   [ServiceFilter(typeof(DevelopmentOnlyFilter))]   
   public async Task<ActionResult<UserViewModel>> Get()
   {
      if (User.Claims.IsNullOrEmpty())
      { 
      
      }
      ValidateSettings();
      if(!ModelState.IsValid) return BadRequest(ModelState);

      var user = await CheckAdminUserAsync();
      var roles = await _usersService.GetRolesAsync(user);
      var model = user.MapViewModel(_mapper);
      model.HasPassword = await _usersService.HasPasswordAsync(user);
      return model;
   }

   [HttpPost]
   [ServiceFilter(typeof(DevelopmentOnlyFilter))] 
   public async Task<ActionResult> Post(AdminRequest request)
   {
      ValidateSettings();
      if(!ModelState.IsValid) return BadRequest(ModelState);

      ValidateRequest(request, _adminSettings);
      if(!ModelState.IsValid) return BadRequest(ModelState);

      if(request.Cmd!.EqualTo(AdminCmds.SetPassword))
      {
         
      }
      else
      {
         ModelState.AddModelError("cmd", "Cmd Not Available.");
      }
      
      if(!ModelState.IsValid) return BadRequest(ModelState);
      return Ok();
   }

   void ValidateSettings()
   {
      if(String.IsNullOrEmpty(_adminSettings.Id)) ModelState.AddModelError("id", "AdminSettings.Id Not Found");
      if(String.IsNullOrEmpty(_adminSettings.Email)) ModelState.AddModelError("email", "AdminSettings.Email Not Found");
      if(String.IsNullOrEmpty(_adminSettings.Key)) ModelState.AddModelError("key", "AdminSettings.Key Not Found");
      if(String.IsNullOrEmpty(_adminSettings.Phone)) ModelState.AddModelError("phone", "AdminSettings.Phone Not Found");
   }

   async Task<User> CheckAdminUserAsync()
   {
      string id = _adminSettings.Id;
      string email = _adminSettings.Email;

      var user = await _usersService.FindByEmailAsync(email);
      if(user == null) throw new AdminSettingsException("User Not Found By Admin.Email.");
      if(id != user.Id) throw new AdminSettingsException("User Id Not Equal To Admin.Id.");

      bool isAdmin = await _usersService.IsAdminAsync(user);
      if(!isAdmin) throw new AdminSettingsException($"User {user.Name} Has No Admin Roles!");

      return user;
   }

}