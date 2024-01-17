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

namespace Web.Controllers;

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
   public async Task<ActionResult<PagedList<Department, DepartmentViewModel>>> Index(bool active, int page = 1, int pageSize = 10)
   {
      var departments =  await _departmentsService.FetchAsync();

      if (departments.HasItems())
      {
         departments = departments.Where(x => x.Active == active);

         departments = departments.GetOrdered().ToList();
      }
      return departments.GetPagedList(_mapper, page, pageSize);
   }


   [HttpGet("create")]
   public ActionResult<DepartmentViewModel> Create() => new DepartmentViewModel();


   [HttpPost]
   public async Task<ActionResult<DepartmentViewModel>> Store([FromBody] DepartmentViewModel model)
   {
      ValidateRequest(model);
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

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] DepartmentViewModel model)
   {
      var department = await _departmentsService.GetByIdAsync(id);
      if (department == null) return NotFound();

      ValidateRequest(model);
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

   void ValidateRequest(DepartmentViewModel model)
   {
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "������g���D");      

   }


}