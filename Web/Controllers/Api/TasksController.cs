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

namespace Web.Controllers.Api;
public class TasksController : BaseApiController
{
   private readonly ITaskService _taskService;

   private readonly IMapper _mapper;

   public TasksController(ITaskService taskService, IMapper mapper)
   {
      _taskService = taskService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<IEnumerable<TaskViewModel>>> Fetch()
   {
      var items = await _taskService.FetchAllAsync();
      return Ok(items.MapViewModelList(_mapper));
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