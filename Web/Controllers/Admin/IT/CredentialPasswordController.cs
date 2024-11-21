using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Web.Models.IT;
using Infrastructure.Services;

namespace Web.Controllers.Admin.IT;

public class CredentialPasswordController : BaseAdminITController
{
   private readonly ICredentialInfoService _credentialInfoService;
   private readonly ICryptoService _cryptoService;
     
   public CredentialPasswordController(ICredentialInfoService credentialInfoService, ICryptoService cryptoService)
   {
      _credentialInfoService = credentialInfoService;
      _cryptoService = cryptoService;
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult<CredentialInfoEditPasswordForm>> Edit(int id)
   {
      var entity = await _credentialInfoService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var model = new CredentialInfoEditPasswordForm();
      model.Id = id;
      model.Username = entity.Username;
      model.Password = "";
      return model;
   }
   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] CredentialInfoEditPasswordForm model)
   {
      var entity = await _credentialInfoService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);     

      entity.Password = _cryptoService.Encrypt(model.Password);

      await _credentialInfoService.UpdateAsync(entity, User.Id());

      return NoContent();
   }

   void ValidateRequest(CredentialInfoEditPasswordForm model)
   {
      var labels = new CredentialInfoLabels();
      if (String.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), ValidationMessages.Required(labels.Password));
   }

}