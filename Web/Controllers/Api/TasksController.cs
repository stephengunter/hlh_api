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
using ApplicationCore.Consts;
using Infrastructure.Entities;
using System;
using ApplicationCore.Authorization;
using ApplicationCore.Exceptions;
using QuestPDF.Helpers;
using Azure.Core;
using ApplicationCore.Services.Files;
using Web.Helpers;
using Newtonsoft.Json;

namespace Web.Controllers.Api;
public class TasksController : BaseApiController
{
   private readonly ITaskService _taskService;
   private readonly IReferenceService _referenceService;
   private readonly IAttachmentService _attachmentService;
   private readonly IMapper _mapper;

   public TasksController(ITaskService taskService, IReferenceService referenceService,
      IAttachmentService attachmentService, IMapper mapper)
   {
      _taskService = taskService;
      _referenceService = referenceService;
      _attachmentService = attachmentService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<TasksIndexModel>> Fetch(int page = 1, int pageSize = 10)
   {
      var tasks = await _taskService.FetchAllAsync();

      var request = new TasksFetchRequest(page, pageSize);
      var actions = new List<string>();
      var model = new TasksIndexModel(request, actions);
      var pagedList = tasks.GetOrdered().GetPagedList(_mapper, page, pageSize);
      

      model.PagedList = pagedList;
      return model;
   }

   [HttpGet("create")]
   public async Task<ActionResult<TaskCreateForm>> Create()
   {
      return Ok(new TaskCreateForm());
   }

   [HttpPost]
   public async Task<ActionResult<TaskViewModel>> Store([FromBody] TaskCreateForm model)
   {
      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new Tasks();
      model.SetValuesTo(entity);
      entity.SetCreated(User.Id());

      entity = await _taskService.CreateAsync(entity);

      return Ok(entity.MapViewModel(_mapper));
   }


   [HttpGet("{id}")]
   public async Task<ActionResult<TaskViewModel>> Details(int id)
   {
      var entity = await _taskService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var refenerces = await _referenceService.FetchAsync(entity);
      if (refenerces.HasItems())
      {
         foreach (var refenerce in refenerces)
         {
            if (refenerce.AttachmentId.HasValue)
            {
               var attachment = await _attachmentService.GetByIdAsync(refenerce.AttachmentId.Value);
               refenerce.Attachment = attachment;
            }
         }
         entity.LoadReferences(refenerces);
      }
     
      return entity.MapViewModel(_mapper);
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<TaskViewModel>> Edit(int id)
   {
      var entity = await _taskService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var refenerces = await _referenceService.FetchAsync(entity);
      if (refenerces.HasItems())
      {
         foreach (var refenerce in refenerces)
         {
            if (refenerce.AttachmentId.HasValue)
            {
               var attachment = await _attachmentService.GetByIdAsync(refenerce.AttachmentId.Value);
               refenerce.Attachment = attachment;
            }
         }
         entity.LoadReferences(refenerces);
      }

      return entity.MapViewModel(_mapper);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] TaskEditForm model)
   {
      var entity = await _taskService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);
      entity.SetUpdated(User.Id());

      await _taskService.UpdateAsync(entity);

      return NoContent();
   }

   async Task ValidateRequestAsync(BaseTaskForm model)
   {
      if(String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫標題");

   }

}