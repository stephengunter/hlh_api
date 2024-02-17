using ApplicationCore.Exceptions;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AutoMapper;
using ApplicationCore.DtoMapper;
using Web.Filters;
using Web.Models;
using ApplicationCore.Consts;
using ApplicationCore.Models;
using Azure.Core;
using ApplicationCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection;
using Infrastructure.Helpers;

namespace Web.Controllers.Admin;

public class DepartmentsController : BaseAdminController
{
   private readonly IDepartmentsService _departmentsService;
   private readonly IMapper _mapper;

  
   public DepartmentsController(IDepartmentsService departmentsService, IMapper mapper)
   {
      _departmentsService = departmentsService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<DepartmentsAdminView>> Index()
   {
      var allDepartments = await _departmentsService.FetchAllAsync();
      var keys = typeof(DepartmentKeys).GetDictionaries<string>();
      var model = new DepartmentsAdminView(allDepartments.MapViewModelList(_mapper), keys);
      return model;
   }


   [HttpGet("create/{parentId}")]
   public async Task<ActionResult<DepartmentViewModel>> Create(int parentId)
   {
      var parent = await _departmentsService.GetByIdAsync(parentId);
      if (parent is null)
      {
         ModelState.AddModelError("parentId", "指定的父部門不存在");
         return BadRequest(ModelState);
      } 

      return new DepartmentViewModel() { ParentId = parentId, Active = true, Parent = parent.MapViewModel(_mapper) };
   }


   [HttpPost]
   public async Task<ActionResult<DepartmentViewModel>> Store([FromBody] DepartmentViewModel model)
   {
      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var department = model.MapEntity(_mapper, User.Id());
      department.Order = model.Active ? 0 : -1;

      department = await _departmentsService.CreateAsync(department);

      return Ok(department.MapViewModel(_mapper));
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult> Edit(int id)
   {
      var department = await _departmentsService.GetByIdAsync(id);
      if (department == null) return NotFound();

      var model = department.MapViewModel(_mapper);

      if (department.ParentId.HasValue)
      {
         var parent = await _departmentsService.GetByIdAsync(department.ParentId.Value);
         model.Parent = parent!.MapViewModel(_mapper);
      }

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] DepartmentViewModel model)
   {
      var department = await _departmentsService.GetByIdAsync(id);
      if (department == null) return NotFound();

      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      department = model.MapEntity(_mapper, User.Id(), department);
      await _departmentsService.UpdateAsync(department);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var department = await _departmentsService.GetByIdAsync(id);
      if (department == null) return NotFound();

      department.Removed = true;
      department.Order = -1;
      await _departmentsService.UpdateAsync(department);

      return NoContent();
   }

   [HttpPut]
   public async Task<IActionResult> Orders([FromBody] OrdersRequest model)
   {
      var departments = await _departmentsService.FetchAsync(model.Ids);
      for (int i = 0; i < model.Ids.Count; i++)
      {
         var department = departments.First(x => x.Id == model.Ids[i]);
         department.Order = model.Ids.Count - i;
      }
      await _departmentsService.UpdateRangeAsync(departments);

      return NoContent();
   }

   async Task ValidateRequestAsync(DepartmentViewModel model)
   {
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫名稱");
      Department? parent = null;
      if (model.ParentId.HasValue)
      {
         parent = await _departmentsService.GetByIdAsync(model.ParentId.Value);
         if (parent is null) ModelState.AddModelError("parentId", "指定的父部門不存在");
      }

      var depatments = await _departmentsService.FetchAsync(parent);
      CheckName(depatments, model);
      CheckKey(depatments, model);
   }

   void CheckName(IEnumerable<Department> departments, DepartmentViewModel model)
   {
      var exist = departments.FindByName(model.Title);
      if (exist != null && exist.Id != model.Id) ModelState.AddModelError("title", "名稱重複了");
   }
   void CheckKey(IEnumerable<Department> departments, DepartmentViewModel model)
   {
      if (model.Key.HasValue())
      {
         var exist = departments.FindByKey(model.Title);
         if (exist != null && exist.Id != model.Id) ModelState.AddModelError("key", "key重複了");
      }
      
   }
   


}