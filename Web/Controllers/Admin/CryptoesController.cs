using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Helpers;
using Infrastructure.Services;
using ApplicationCore.Models.IT;

namespace Web.Controllers.Admin;

public class CryptoesController : BaseAdminController
{
   private readonly ICryptoService _cryptoService;
   private readonly ICredentialInfoService _credentialInfoService;
   public CryptoesController(ICryptoService cryptoService, ICredentialInfoService credentialInfoService)
   {
      _credentialInfoService = credentialInfoService;
      _cryptoService = cryptoService;
   }

   [HttpGet("{type}/{id}")]
   public async Task<ActionResult<string>> Get(string type, int id)
   {
      if (type.EqualTo(nameof(CredentialInfo)))
      {
         var credentialInfo = await _credentialInfoService.GetByIdAsync(id);
         if (credentialInfo == null) return NotFound();

         if (String.IsNullOrEmpty(credentialInfo.Password))
         {
            ModelState.AddModelError(nameof(CredentialInfo.Password), ValidationMessages.IsEmpty(nameof(CredentialInfo.Password)));
            return BadRequest(ModelState);
         }

         return _cryptoService.Decrypt(credentialInfo.Password);
      }
      else
      {
         ModelState.AddModelError("type", ValidationMessages.NotExist($"EntityType: {type}"));
         return BadRequest(ModelState);
      }
   }

}