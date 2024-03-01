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

public class LocationsController : BaseAdminController
{
   private readonly ILocationsService _locationsService;
   private readonly IMapper _mapper;
   private readonly DefaultContext _context;
   private readonly AdminSettings _adminSettings;
   private readonly DbSettings _dbSettings;

   public LocationsController(ILocationsService locationsService, IJobsService jobsService,
      IWebHostEnvironment environment, DefaultContext context,
      IOptions<AdminSettings> adminSettings, IOptions<DbSettings> dbSettings, IMapper mapper)
   {
      _locationsService = locationsService;
      _context = context;
      _mapper = mapper;
      _dbSettings = dbSettings.Value;
      _adminSettings = adminSettings.Value;
   }

   [HttpGet]
   public async Task<ActionResult<IEnumerable<LocationViewModel>>> Index()
   {
      var allLocations = await _locationsService.FetchAllAsync();
      return allLocations.MapViewModelList(_mapper);
   }


   [HttpGet("create/{parentId}")]
   public async Task<ActionResult<LocationViewModel>> Create(int parentId)
   {
      var parent = await _locationsService.GetByIdAsync(parentId);
      if (parent is null)
      {
         ModelState.AddModelError("parentId", "指定的父位置不存在");
         return BadRequest(ModelState);
      } 

      return new LocationViewModel() { ParentId = parentId, Active = true, Parent = parent.MapViewModel(_mapper) };
   }


   [HttpPost]
   public async Task<ActionResult<LocationViewModel>> Store([FromBody] LocationViewModel model)
   {
      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var location = model.MapEntity(_mapper, User.Id());
      location.Order = model.Active ? 0 : -1;

      location = await _locationsService.CreateAsync(location);

      return Ok(location.MapViewModel(_mapper));
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult> Edit(int id)
   {
      var location = await _locationsService.GetByIdAsync(id);
      if (location == null) return NotFound();

      var model = location.MapViewModel(_mapper);

      if (location.ParentId.HasValue)
      {
         var parent = await _locationsService.GetByIdAsync(location.ParentId.Value);
         model.Parent = parent!.MapViewModel(_mapper);
      }

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] LocationViewModel model)
   {
      var location = await _locationsService.GetByIdAsync(id);
      if (location == null) return NotFound();

      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      location = model.MapEntity(_mapper, User.Id(), location);
      await _locationsService.UpdateAsync(location);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var location = await _locationsService.GetByIdAsync(id);
      if (location == null) return NotFound();

      location.Removed = true;
      location.Order = -1;
      await _locationsService.UpdateAsync(location);

      return NoContent();
   }

   [HttpPut]
   public async Task<IActionResult> Orders([FromBody] OrdersRequest model)
   {
      var locations = await _locationsService.FetchAsync(model.Ids);
      for (int i = 0; i < model.Ids.Count; i++)
      {
         var location = locations.First(x => x.Id == model.Ids[i]);
         location.Order = model.Ids.Count - i;
      }
      await _locationsService.UpdateRangeAsync(locations);

      return NoContent();
   }
   [HttpPost("export")]
   public async Task<ActionResult> Export([FromBody] AdminRequest request)
   {
      ValidateRequest(request, _adminSettings);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var allLocations = await _locationsService.FetchAllAsync();
      foreach (var location in allLocations)
      {
         location.Parent = null;
         location.SubItems = new List<Location>();
      } 
      var viewList = allLocations.MapViewModelList(_mapper);

      // Serialize users data to JSON format
      string jsonData = JsonConvert.SerializeObject(viewList);

      // Convert JSON data to bytes
      byte[] fileBytes = Encoding.UTF8.GetBytes(jsonData);

      // Return the JSON data as a downloadable file
      return File(fileBytes, "application/json", "locations.json");
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
      var viewList = JsonConvert.DeserializeObject<List<LocationViewModel>>(content);

      var connectionString = _context.Database.GetDbConnection().ConnectionString;

      var newEntities = new List<Location>();
      foreach (var viewModel in viewList!)
      {
         var existingEntity = _context.Locations.Find(viewModel.Id);
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
         var tableName = _context.Model.FindEntityType(typeof(Location))!.GetTableName();
         using (var context = Factories.CreateDefaultContext(connectionString, _dbSettings))
         {
            context.Locations.AddRange(newEntities);
            // context.Locations
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

   async Task ValidateRequestAsync(LocationViewModel model)
   {
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫名稱");
      Location? parent = null;
      if (model.ParentId.HasValue)
      {
         parent = await _locationsService.GetByIdAsync(model.ParentId.Value);
         if (parent is null) ModelState.AddModelError("parentId", "指定的父位置不存在");
      }

      var depatments = await _locationsService.FetchAsync(parent);
      CheckName(depatments, model);

      depatments = await _locationsService.FetchAllAsync();
      CheckKey(depatments, model);
   }

   void CheckName(IEnumerable<Location> locations, LocationViewModel model)
   {
      var exist = locations.FindByName(model.Title);
      if (exist != null && exist.Id != model.Id) ModelState.AddModelError("title", "名稱重複了");
   }
   void CheckKey(IEnumerable<Location> locations, LocationViewModel model)
   {
      if (model.Key.HasValue())
      {
         var exist = locations.FindByKey(model.Key);
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