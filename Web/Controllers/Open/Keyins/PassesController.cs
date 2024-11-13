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
   public async Task<ActionResult<ICollection<KeyinPersonView>>> Index()
   {
      var persons = await _personService.FetchAllPassAsync();
      return persons.MapViewModelList(_mapper);
   }

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] PassesPersonAddRequest request)
   {
      foreach (var item in request.Persons)
      {
         var entity = await _personService.FindByNameAsync(item.Name);

         if (entity != null)
         {
            entity.HighRun = item.HighRun;
            entity.AllPass = true;
            await _personService.UpdateAsync(entity);
         }
         else
         {
            entity = item.MapEntity(_mapper);
            entity.AllPass = true;
            entity = await _personService.CreateAsync(entity);
         }
      }
      return Ok();

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
}