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
   private readonly AttachmentSettings _attachmentSettings;
   private readonly ITaskService _taskService;
   private readonly IFileStoragesService _fileStoragesService;
   private readonly IAttachmentService _attachmentService;
   private readonly IMapper _mapper;

   public TasksController(IOptions<AttachmentSettings> attachmentSettings, ITaskService taskService, IAttachmentService attachmentService,
      IMapper mapper)
   {
      _attachmentSettings = attachmentSettings.Value;
      _attachmentService = attachmentService;
      _taskService = taskService;
      _mapper = mapper;

      this.InitFileStoragesService(_fileStoragesService, _attachmentSettings);

      //if (String.IsNullOrEmpty(_attachmentSettings.Host))
      //{
      //   _fileStoragesService = new LocalStoragesService(_attachmentSettings.Directory);
      //}
      //else
      //{
      //   _fileStoragesService = new FtpStoragesService(_attachmentSettings.Host, _attachmentSettings.UserName,
      //   _attachmentSettings.Password, _attachmentSettings.Directory);
      //}
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

      var references = new List<Reference>();

      foreach (var referenceForm in model.References)
      {
         var reference = new Reference() { Title = referenceForm.Title };
         if (referenceForm.File != null)
         {
            var attchment = this.SaveAttamentFile(referenceForm.File, _fileStoragesService);
            attchment.PostType = PostType.Reference;
            attchment.SetCreated(User.Id());
            attchment = await _attachmentService.CreateAsync(attchment);

            reference.AttachmentId = attchment.Id;
         }
         else 
         {
            reference.Url = referenceForm.Url!;
         }
         references.Add(reference);
      }
      
      if (references.HasItems()) entity.References = JsonConvert.SerializeObject(references);

      entity = await _taskService.CreateAsync(entity);

      return Ok(entity.MapViewModel(_mapper));
   }


   [HttpGet("{id}")]
   public async Task<ActionResult<TaskViewModel>> Details(int id)
   {
      var entity = await _taskService.GetByIdAsync(id);
      if (entity == null) return NotFound();


      return entity.MapViewModel(_mapper);
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<TaskEditForm>> Edit(int id)
   {
      var entity = await _taskService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var model = new TaskEditForm();
      entity.SetValuesTo(model);

      return model;
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