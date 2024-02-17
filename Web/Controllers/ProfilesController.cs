using ApplicationCore.Services;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Helpers;
using Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers;

[EnableCors("Global")]
[Authorize]
public class ProfilesController : BaseController
{
   private readonly IUsersService _usersService;
   private readonly IMapper _mapper;

   public ProfilesController(IUsersService usersService, IMapper mapper)
   {
      _usersService = usersService;
      _mapper = mapper;
   }

   [HttpGet("{id}")]   
   public async Task<ActionResult<UserViewModel>> Get(string id)
   {
      var user = await _usersService.FindByIdAsync(id);
      if (user == null) return NotFound();

      CheckCurrentUser(user);

      var roles = await _usersService.GetRolesAsync(user);
      var model = user.MapViewModel(_mapper);
      model.HasPassword = await _usersService.HasPasswordAsync(user);
      return model;
   }

   [HttpPut("{id}")]
   public async Task<IActionResult> Update(string id, UserViewModel model)
   {
      var user = await _usersService.FindByIdAsync(id);
      if (user == null) return NotFound();

      if (String.IsNullOrEmpty(model.Name)) ModelState.AddModelError("name", "name can not be empty!");
      if (!ModelState.IsValid) return BadRequest(ModelState);

      user.Name = model.Name!;
      user.PhoneNumber = model.PhoneNumber;

      await _usersService.UpdateAsync(user);      
      return NoContent();
   }

}