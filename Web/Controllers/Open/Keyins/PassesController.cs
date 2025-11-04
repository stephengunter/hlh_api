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

namespace Web.Controllers.Open.Keyins;

[Route("open/keyins/[controller]")]
public class PassesController : BaseOpenController
{
   private readonly IMapper _mapper;
   private readonly IKeyinPersonService _personService;
   public PassesController(IKeyinPersonService personService, IMapper mapper)
   {
      _personService = personService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<ICollection<KeyinPersonView>>> Index(int pass)
   {
      IEnumerable<KeyinPerson> persons = null;
      if(pass > 0) persons = await _personService.FetchAllPassAsync();
      else persons = await _personService.FetchAsync();
      return persons.MapViewModelList(_mapper);
   }

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] KeyinPersonView form)
   {
      ValidateRequest(form, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new KeyinPerson();
      form.SetValuesTo(entity);

      entity = await _personService.CreateAsync(entity);

      return Ok(entity.MapViewModel(_mapper));

   }

   [HttpPost("upload")]
   public async Task<ActionResult<ICollection<KeyinPersonView>>> Upload([FromForm] PassesPersonUploadRequest request)
   {
      var file = request.File;
      // Check if the file is a .txt file
      if (file == null || Path.GetExtension(file.FileName).ToLower() != ".txt")
      {
         return BadRequest("Invalid file format. Please upload a .txt file.");
      }
      var persons = new List<KeyinPersonView>();
      using (var streamReader = new StreamReader(file.OpenReadStream()))
      {
         string line;
         while ((line = await streamReader.ReadLineAsync()) != null)
         {
            var values = line.Split(',');
            if (values.Length == 5 && values[0] == "臺灣高等法院花蓮分院")
            {
               var person = new KeyinPersonView()
               {
                  Account = values[1],
                  Name = values[2],
                  HighRun = values[3].ToInt()
               };
               persons.Add(person);
            }
         }
      }
      return persons;
   }
   [HttpGet("create")]
   public ActionResult<KeyinPersonView> Create() => new KeyinPersonView();

   [HttpGet("edit/{id}")]
   public async Task<ActionResult<KeyinPersonView>> Edit(int id)
   {
      var entity = await _personService.GetByIdAsync(id);
      if (entity == null) return NotFound();
      
      var form = entity.MapViewModel(_mapper);
      return form;
   }
   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] KeyinPersonView model)
   {
      var entity = await _personService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      ValidateRequest(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);
      entity.LeaveAt = model.LeaveAtText.ToDatetimeOrNull();
      entity.AllPass = model.HighRun > 0;


      await _personService.UpdateAsync(entity);

      return NoContent();
   }
   [HttpPost("reports")]
   public async Task<IActionResult> Reports(PersonPassesReportRequest request)
   {
      var persons = await _personService.FetchAllPassAsync();
      var views = persons.MapViewModelList(_mapper);
     

      var items = views.MapReportItemList();
      string title = $"免測人員";
      var model = new PersonPassesReportModel(title, request, items);
      var doc = new PersonPassesReportDocument(model);

      byte[] bytes = doc.GeneratePdf();
      return Ok(new BaseFileView(title, bytes));
   }
   void ValidateRequest(KeyinPersonView model, int id)
   {
      var labels = new KeyinPersonLabels();
      if (String.IsNullOrEmpty(model.Account)) ModelState.AddModelError(nameof(model.Account), ValidationMessages.Required(labels.Account));
      if (String.IsNullOrEmpty(model.Name)) ModelState.AddModelError(nameof(model.Name), ValidationMessages.Required(labels.Name));
      if (String.IsNullOrEmpty(model.Unit)) ModelState.AddModelError(nameof(model.Unit), ValidationMessages.Required(labels.Unit));
     
   }
}