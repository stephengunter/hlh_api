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
using ApplicationCore.DataAccess;
using Newtonsoft.Json;
using System.Text;
using ApplicationCore;

namespace Web.Controllers.Admin;

public class DepartmentsController : BaseAdminController
{
   private readonly IDepartmentsService _departmentsService;
   private readonly IJobsService _jobsService;
   private readonly IMapper _mapper;
   private readonly IWebHostEnvironment _environment;
   private readonly DefaultContext _context;
   private readonly AdminSettings _adminSettings;
   private readonly DbSettings _dbSettings;

   public DepartmentsController(IDepartmentsService departmentsService, IJobsService jobsService,
      IWebHostEnvironment environment, DefaultContext context,
      IOptions<AdminSettings> adminSettings, IOptions<DbSettings> dbSettings, IMapper mapper)
   {
      _departmentsService = departmentsService;
      _jobsService = jobsService;
      _environment = environment;
      _context = context;
      _mapper = mapper;
      _dbSettings = dbSettings.Value;
      _adminSettings = adminSettings.Value;
   }

   [HttpGet]
   public async Task<ActionResult<DepartmentsAdminView>> Index()
   {
      var allDepartments = await _departmentsService.FetchAllAsync();
      var keys = typeof(DepartmentKeys).GetDictionaries<string>();
      var model = new DepartmentsAdminView(allDepartments.MapViewModelList(_mapper), keys);

      var jotTitles = await _jobsService.FetchJobTitlesAsync();
      model.JobTitles = jotTitles.Select(x => x.MapViewModel(_mapper)).ToList();
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
   [HttpPost("export")]
   public async Task<ActionResult> Export([FromBody] AdminRequest request)
   {
      ValidateRequest(request, _adminSettings);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var path = GetTempPath(_environment, DateTime.Today.ToDateNumber().ToString());
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);

      var allDepartments = await _departmentsService.FetchAllAsync();
      foreach (var department in allDepartments)
      {
         department.Parent = null;
         department.SubItems = new List<Department>();
      } 
      var viewList = allDepartments.MapViewModelList(_mapper);

      // Serialize users data to JSON format
      string jsonData = JsonConvert.SerializeObject(viewList);

      // Convert JSON data to bytes
      byte[] fileBytes = Encoding.UTF8.GetBytes(jsonData);

      // Return the JSON data as a downloadable file
      return File(fileBytes, "application/json", "departments.json");
   }
   [HttpPost("import")]
   public async Task<ActionResult> Import([FromForm] AdminFileRequest request)
   {
      ValidateRequest(request, _adminSettings);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      if (request.Files.Count < 1)
      {
         ModelState.AddModelError("files", "必須上傳檔案");
         return BadRequest(ModelState);
      }


      var file = request.Files.FirstOrDefault();
      if (Path.GetExtension(file!.FileName).ToLower() != ".json")
      {
         ModelState.AddModelError("files", "檔案格式錯誤");
         return BadRequest(ModelState);
      }

      string content = await ReadFileTextAsync(file);
      var viewList = JsonConvert.DeserializeObject<List<DepartmentViewModel>>(content);

      var connectionString = _context.Database.GetDbConnection().ConnectionString;

      var newEntities = new List<Department>();
      foreach (var viewModel in viewList!)
      {
         var existingEntity = _context.Departments.Find(viewModel.Id);
         if (existingEntity == null) newEntities.Add(viewModel.MapEntity(_mapper, User.Id()));
         else 
         {
            var entry = _context.Entry(existingEntity);
            entry.CurrentValues.SetValues(viewModel.MapEntity(_mapper, User.Id()));
            entry.State = EntityState.Modified;
         }
      }

      _context.SaveChanges();

      if (newEntities.HasItems())
      {
         var tableName = _context.Model.FindEntityType(typeof(Department))!.GetTableName();
         using (var context = Factories.CreateDefaultContext(connectionString, _dbSettings))
         {
            context.Departments.AddRange(newEntities);
            // context.Departments
            context.Database.OpenConnection();
            try
            {
               context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} ON");
               context.SaveChanges();
               context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT {tableName} OFF");
            }
            finally
            {
               context.Database.CloseConnection();
            }
         }
      }



      return Ok();
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

      depatments = await _departmentsService.FetchAllAsync();
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
         var exist = departments.FindByKey(model.Key);
         if (exist != null && exist.Id != model.Id) ModelState.AddModelError("key", "key重複了");
      }
      
   }
   async Task<string> ReadFileTextAsync(IFormFile file)
   {
      var result = new StringBuilder();
      using (var reader = new StreamReader(file.OpenReadStream()))
      {
         while (reader.Peek() >= 0) result.AppendLine(await reader.ReadLineAsync());
      }
      return result.ToString();

   }


}