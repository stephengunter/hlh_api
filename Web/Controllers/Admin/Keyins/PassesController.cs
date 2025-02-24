using ApplicationCore.Helpers;
using ApplicationCore.Helpers.Keyin;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Helpers;
using ApplicationCore.Services.Keyin;
using ApplicationCore.Views.Keyin;
using Web.Models.Keyin;
using ApplicationCore.Models.Keyin;
using OfficeOpenXml;
using Infrastructure.Views;
using QuestPDF.Fluent;
using Microsoft.Identity.Client;
using Web.Models.IT;
using ApplicationCore.Authorization;
using ApplicationCore.Models;

namespace Web.Controllers.Admin;

[Route("admin/keyins/[controller]")]
public class PassesController : BaseAdminController
{
   private readonly IMapper _mapper;
   private readonly IKeyinPersonService _personService;
   public PassesController(IKeyinPersonService personService, IMapper mapper)
   {
      _personService = personService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<ICollection<KeyinPersonView>>> Index(int pass = 0)
   {
      if (pass > 0)
      {
         var persons = await _personService.FetchAllPassAsync();
         persons = persons.Where(x => x.IsActive(DateTime.Today.Year, DateTime.Today.Month));
         return persons.MapViewModelList(_mapper);
      }
      else
      { 
         var persons = await _personService.FetchAsync();
         return persons.MapViewModelList(_mapper);
      } 
   }
   
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<KeyinPersonView>> Edit(int id)
   {
      var entity = await _personService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var form = new KeyinPersonView();
      entity.SetValuesTo(form);
      form.LeaveAtText = entity.LeaveAt.ToDateString();

      return form;
   }
   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] KeyinPersonView model)
   {
      var entity = await _personService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);
      if(entity.HighRun > 0) entity.AllPass = true;
      else entity.AllPass = false;

      entity.LeaveAt = model.LeaveAtText.ToDatetimeOrNull();

      await _personService.UpdateAsync(entity);

      return NoContent();
   }

   [HttpPost]
   public async Task<ActionResult<KeyinPersonView>> Store([FromBody] KeyinPersonView model)
   {
      await ValidateRequestAsync(model, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new KeyinPerson();
      model.SetValuesTo(entity);
      if (entity.HighRun > 0) entity.AllPass = true;
      else entity.AllPass = false;

      entity.LeaveAt = model.LeaveAtText.ToDatetimeOrNull();

      await _personService.CreateAsync(entity);

      return entity.MapViewModel(_mapper);

   }


   [HttpPost("reports")]
   public async Task<IActionResult> Reports(PersonPassesReportRequest request)
   {
      var persons = await _personService.FetchAllPassAsync();
      persons = persons.Where(x => x.IsActive(DateTime.Today.Year, DateTime.Today.Month));
      var views = persons.MapViewModelList(_mapper);


      var items = views.MapReportItemList();
      string title = $"免測人員";
      var model = new PersonPassesReportModel(title, request, items);
      var doc = new PersonPassesReportDocument(model);

      byte[] bytes = doc.GeneratePdf();
      return Ok(new BaseFileView(title, bytes));
   }
   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _personService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await _personService.RemoveAsync(entity);

      return NoContent();
   }

   async Task ValidateRequestAsync(KeyinPersonView model, int id)
   {
      if (String.IsNullOrEmpty(model.Name)) ModelState.AddModelError("Name", "必須填寫Name");
      if (String.IsNullOrEmpty(model.Unit)) ModelState.AddModelError("Unit", "必須填寫Unit");
      if (String.IsNullOrEmpty(model.Account)) ModelState.AddModelError("Account", "必須填寫Account");
      if (!ModelState.IsValid) return;
      
      var exist = await _personService.FindByNameAsync(model.Name);
      if (exist != null && exist.Id != model.Id) ModelState.AddModelError("Name", "Name重複了");
   }
}