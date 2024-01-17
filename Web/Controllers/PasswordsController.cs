using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers;

[EnableCors("Global")]
[Authorize]
public class PasswordsController : BaseController
{
   private readonly UserManager<User> _userManager;

   public PasswordsController(UserManager<User> userManager)
   {
      _userManager = userManager;
   }

   [HttpPost]
   public async Task<ActionResult> Store(SetPasswordRequest request)
   {
      var user = await _userManager.FindByIdAsync(request.UserId);
      if (user == null) return NotFound();

      CheckCurrentUser(user);

      if (String.IsNullOrEmpty(request.Password)) ModelState.AddModelError("password", "Password Can Not Be Empty.");
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _userManager.AddPasswordAsync(user, request.Password);
      if (result.Succeeded) return Ok();

      //var error = result.Errors.FirstOrDefault();
      //string msg = $"{error!.Code} : {error!.Description}" ?? string.Empty;
      ModelState.AddModelError("", "密碼設定失敗");
      return BadRequest(ModelState);
   }

   [HttpPut("{id}")]
   public async Task<IActionResult> Update(string id, SetPasswordRequest request)
   {
      var user = await _userManager.FindByIdAsync(id);
      if(user == null) return NotFound();

      CheckCurrentUser(user);

      if (String.IsNullOrEmpty(request.Password)) ModelState.AddModelError("password", "Password Can Not Be Empty.");
      if (String.IsNullOrEmpty(request.Token)) ModelState.AddModelError("token", "token can not be empty!");
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var result = await _userManager.ChangePasswordAsync(user, request.Token, request.Password);
      if (result.Succeeded) return NoContent();

      //var error = result.Errors.FirstOrDefault();
      //string msg = $"{error!.Code} : {error!.Description}" ?? string.Empty;
      ModelState.AddModelError("", "密碼變更失敗");
      return BadRequest(ModelState);
   }

}