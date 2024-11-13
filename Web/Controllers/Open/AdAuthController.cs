using Microsoft.AspNetCore.Mvc;
using Web.Models;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Google.Apis.Auth;
using ApplicationCore.Consts;
using Microsoft.AspNetCore.Cors;
using ApplicationCore.Models.Auth;
using ApplicationCore.Services.Auth;
using ApplicationCore.Settings;
using ApplicationCore;
using Microsoft.Extensions.Options;
using ApplicationCore.Views.AD;

namespace Web.Controllers.Open;

public class AdAuthController : BaseOpenController
{
   private readonly ILdapService _ldapService;

   public AdAuthController(IOptions<LdapSettings> ldapSettings)
   {
      _ldapService = Factories.CreateLdapService(ldapSettings.Value);
   }


   [HttpPost]
   public async Task<ActionResult<AdUser?>> Login([FromBody] LoginRequest model)
   {
      var adUser = _ldapService.CheckAuth(model.Username.Trim(), model.Password.Trim());
      return adUser;
   }




}
