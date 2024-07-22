using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Web.Models;
using ApplicationCore.Services.Files;
using Azure.Core;
using Ardalis.Specification;
using ApplicationCore.Exceptions;
using ApplicationCore.Authorization;
using ApplicationCore.Consts;

namespace Web.Controllers.Api;

public class ReferencesController : BaseApiController
{
   private readonly IReferenceService _referenceService;
   private readonly IAttachmentService _attachmentService;
   private readonly IMapper _mapper;

   public ReferencesController(IReferenceService referenceService, IAttachmentService attachmentService, IMapper mapper)
   {
      _referenceService = referenceService;
      _attachmentService = attachmentService;
      _mapper = mapper;
   }

   [HttpPost]
   public async Task<ActionResult<ReferenceViewModel>> Store([FromBody] ReferenceCreateForm model)
   {
      var entity = new Reference();     
      
      model.SetValuesTo(entity);
      
      entity.SetCreated(User.Id());

      entity = await _referenceService.CreateAsync(entity);

      await SyncAttachments(entity);

      return entity.MapViewModel(_mapper);
   }



   [HttpGet("{id}")]
   public async Task<ActionResult> Get(int id)
   {
      var entity = await _referenceService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var model = entity.MapViewModel(_mapper);
      return Ok(model);
   }


   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] ReferenceEditForm model)
   {
      var entity = await _referenceService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      model.SetValuesTo(entity);
      entity.SetUpdated(User.Id());

      await _referenceService.UpdateAsync(entity);

      await SyncAttachments(entity);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(string id)
   {
      var ids = id.SplitToIds();
      if (ids.IsNullOrEmpty())
      {
         ModelState.AddModelError("id", "¿ù»~ªºid");
         return BadRequest(ModelState);
      }

      foreach (int entityId in ids)
      {
         var entity = await _referenceService.GetByIdAsync(entityId);
         if (entity == null)
         {
            ModelState.AddModelError("id", $"¿ù»~ªºid: {entityId}");
            return BadRequest(ModelState);
         }

         entity.Removed = true;
         entity.Order = -1;
         entity.SetUpdated(User.Id());
         await _referenceService.UpdateAsync(entity);

         await SyncAttachments(entity);
      }

      
      return NoContent();
   }

   async Task SyncAttachments(Reference entity)
   {
      if (entity.AttachmentId.HasValue && entity.AttachmentId.Value > 0)
      {
         var attachment = await _attachmentService.GetByIdAsync(entity.AttachmentId.Value);
         attachment!.PostType = PostTypes.Reference;
         attachment.PostId = entity.Id;
         attachment.Removed = entity.Removed;
         attachment.SetUpdated(User.Id());
         await _attachmentService.UpdateAsync(attachment);
      }

      var attachments = await _attachmentService.FetchAsync(entity);
      if (entity.AttachmentId.HasValue && entity.AttachmentId.Value > 0)
      {
         attachments = attachments.Where(x => x.Id != entity.AttachmentId.Value);
      }

      if (attachments.HasItems())
      {
         foreach (var item in attachments)
         {
            item.Removed = true;
            item.SetUpdated(User.Id());
         }
         await _attachmentService.UpdateRangeAsync(attachments);
      }
   }

}