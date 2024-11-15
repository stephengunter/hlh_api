using ApplicationCore.Services;
using ApplicationCore.Views.IT;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Web.Models.IT;
using ApplicationCore.Models.IT;
using Infrastructure.Services;

namespace Web.Controllers.Admin.IT;

public class CredentialInfoesController : BaseAdminITController
{
   private readonly ICredentialInfoService _credentialInfoService;
   private readonly IHostService _hostService;
   private readonly ICryptoService _cryptoService;
   private readonly IMapper _mapper;
     
   public CredentialInfoesController(ICredentialInfoService credentialInfoService, 
      IHostService databaseService, ICryptoService cryptoService, IMapper mapper)
   {
      _credentialInfoService = credentialInfoService;
      _hostService = databaseService;
      _cryptoService = cryptoService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<ICollection<CredentialInfoViewModel>>> Index(string entity, int entityId)
   {
      if (String.IsNullOrEmpty(entity))
      {
         ModelState.AddModelError("entity", ValidationMessages.Required("entity"));
         return BadRequest(ModelState);
      }
      if (entity != nameof(ApplicationCore.Models.IT.Host))
      {
         ModelState.AddModelError("entity", ValidationMessages.NotExist($"EntityType: {entity}"));
         return BadRequest(ModelState);
      } 
      var host = await _hostService.GetByIdAsync(entityId);
      if (host == null)
      {
         ModelState.AddModelError("entityId", ValidationMessages.NotExist($"EntityId: {entityId}"));
         return BadRequest(ModelState);
      }
      var list = await _credentialInfoService.FetchAsync(host);
      return list.MapViewModelList(_mapper);
   }
   [HttpGet("create")]
   public ActionResult<CredentialInfoAddForm> Create() => new CredentialInfoAddForm();


   [HttpPost]
   public async Task<ActionResult<CredentialInfoViewModel>> Store([FromBody] CredentialInfoAddForm model)
   {
      await ValidateRequest(model, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new CredentialInfo();
      model.SetValuesTo(entity);

      entity.Password = _cryptoService.Encrypt(model.Password);

      entity = await _credentialInfoService.CreateAsync(entity, User.Id());

      return Ok(entity.MapViewModel(_mapper));
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult> Edit(int id)
   {
      var entity = await _credentialInfoService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var model = entity.MapViewModel(_mapper);

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] CredentialInfoEditForm model)
   {
      var entity = await _credentialInfoService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequest(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);
      entity.Password = _cryptoService.Encrypt(model.Password);

      await _credentialInfoService.UpdateAsync(entity, User.Id());

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _credentialInfoService.GetByIdAsync(id);
      if (entity == null) return NotFound();
     
      await _credentialInfoService.RemoveAsync(entity, User.Id());

      return NoContent();
   }

   async Task ValidateRequest(CredentialInfoBaseForm model, int id)
   {
      var labels = new CredentialInfoLabels();
      if (String.IsNullOrEmpty(model.Username)) ModelState.AddModelError(nameof(model.Username), ValidationMessages.Required(labels.Username));
      if (String.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), ValidationMessages.Required(labels.Password));
      if (String.IsNullOrEmpty(model.EntityType)) ModelState.AddModelError(nameof(model.EntityType), ValidationMessages.Required(nameof(model.EntityType)));

      if (model.EntityType.EqualTo(nameof(ApplicationCore.Models.IT.Host)))
      {
         var host = await _hostService.GetByIdAsync(model.EntityId);
         if (host == null)
         {
            ModelState.AddModelError(nameof(model.EntityId), ValidationMessages.NotExist($"EntityId: {model.EntityId}"));
            return;
         }
         var list = await _credentialInfoService.FetchAsync(host);
         if (list.HasItems())
         {
            var exist = list.FirstOrDefault(x => x.Username == model.Username);
            if (exist != null && exist.Id != id)
            {
               ModelState.AddModelError(nameof(model.Username), ValidationMessages.Duplicate(labels.Username));
            }
         }
      }
      else
      {
         ModelState.AddModelError(nameof(model.EntityType), ValidationMessages.NotExist($"EntityType: {model.EntityType}"));
      }
      
   }


}