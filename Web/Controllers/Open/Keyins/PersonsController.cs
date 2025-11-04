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
using System.Text;

namespace Web.Controllers.Open.Keyins;

[Route("open/keyins/[controller]")]
public class PersonsController : BaseOpenController
{
   private readonly IMapper _mapper;
   private readonly IKeyinPersonService _personService;
   private readonly IPersonRecordService _recordsService;
   public PersonsController(IKeyinPersonService personService,
      IPersonRecordService recordsService, IMapper mapper)
   {
      _personService = personService;
      _recordsService = recordsService;
      _mapper = mapper;
   }

   [HttpGet("init")]
   public async Task<ActionResult<PersonRecordsIndexModel>> Init()
   {
      var today = DateTime.Today;
      int year = today.Year - 1911;
      int month = today.Month - 1;
     
      var minYear = await _recordsService.FindMinYearAsync();
      if (!minYear.HasValue) minYear = year - 1;
      var years = new List<int>();
      for (int i = minYear.Value; i <= year; i++)
      {
         years.Add(i);
      }

      var request = new PersonRecordsFetchRequest(year, month);
      var persons = await _personService.FetchAsync();

      return new PersonRecordsIndexModel(years, request, persons.MapViewModelList(_mapper));
   }
   [HttpGet]
   public async Task<ActionResult<ICollection<PersonRecordView>>> Index(int year, int month)
   {
      var records = await _recordsService.FetchAsync(year, month);
      records = records.OrderByDescending(r => r.Score).ToList();

      var persons = await _personService.FetchAsync();
      var views = new List<PersonRecordView>();
      foreach (var record in records)
      {
         record.Person = persons.FirstOrDefault(x => x.Id == record.PersonId);
         views.Add(record.MapViewModel(_mapper));
      }
      return views;
   }

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] PersonRecordsAddRequest request)
   {
      int year = request.Year;
      int month = request.Month;
      foreach (var item in request.Records)
      {
         var entity = await _recordsService.FindAsync(new KeyinPerson { Id = item.PersonId }, year, month);

         if (entity != null)
         {
            entity.Score = item.Score;
            entity.CorrectRate = item.CorrectRate.ToDouble();
            entity.Unit = item.Unit;
            await _recordsService.UpdateAsync(entity);
         }
         else
         {
            item.Month = month;
            item.Year = year;

            entity = await _recordsService.CreateAsync(item.MapEntity(_mapper));
         }

         var prevMonth = month - 1;
         var prevRecord = await _recordsService.FindAsync(new KeyinPerson { Id = item.PersonId },
                                                               prevMonth < 1 ? (year - 1) : year,
                                                               prevMonth < 1 ? 12 : prevMonth
                                                               );
         if (prevRecord != null && entity.Score != 0 && prevRecord.Score != 0)
         {
            double increaseRate = ((double)(entity.Score - prevRecord.Score) / prevRecord.Score) * 100;
            entity.Diff = Math.Round(increaseRate, 2).ToString();
            await _recordsService.UpdateAsync(entity);
         }
      }
      return Ok();

   }
   [HttpPost("upload")]
   public async Task<ActionResult<ICollection<PersonRecordView>>> Upload([FromForm] PersonRecordsUploadRequest request)
   {
      int year = request.Year;
      int month = request.Month;
      var file = request.File;
      var errors = ValidateFile(file!);
      AddErrors(errors);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      var records = new List<PersonRecord>();
      using (var stream = new MemoryStream())
      {
         await file!.CopyToAsync(stream);
         stream.Position = 0;
         using (var reader = new StreamReader(stream, Encoding.GetEncoding(950)))
         {
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
               var parts = line.Split(',');
               int columns = parts.Length;
               string unit = parts[0];
               if (string.IsNullOrEmpty(unit)) continue;
               unit = unit.Trim();
               if (unit.Length > 2) continue;
               if (unit == "平均") break;
               if (!unit.EndsWith("股")) continue;
               

               string account = parts[1].Trim();
               if (!IsValidAccount(account)) continue;

               string name = parts[2].Trim();

               int index = columns - 4;
               int score = parts[index].ToInt();

               index = columns - 2;
               double correctRate = parts[index].ToDouble();

               var person = await _personService.FindByNameAsync(name);
               if (person == null)
               {
                  person = await _personService.CreateAsync(new KeyinPerson
                  {
                     Name = name,
                     Unit = unit,
                     Account = account
                  });
               }
               else
               {
                  if (person.IsActive(year + 1911, month))
                  {
                     if (person.Unit != unit)
                     {
                        person.Unit = unit;
                        await _personService.UpdateAsync(person);
                     }
                  }
                  else continue;
               }
               var record = new PersonRecord()
               {
                  Unit = unit,
                  Score = score,
                  CorrectRate = correctRate,
                  PersonId = person.Id,
                  Person = person
               };
               records.Add(record);

            }
         }
      }
      return records.MapViewModelList(_mapper);
   }

   
   bool IsValidAccount(string input)
   {
      if (string.IsNullOrEmpty(input)) return false;
      string val = input.Trim();
      if (string.IsNullOrEmpty(val)) return false;
      if (val.Length != 6) return false;
      if (val.FirstOrDefault().ToString().ToUpper() != "U") return false;
      return true;
   }
   [HttpPost("reports")]
   public async Task<IActionResult> Reports(PersonRecordReportRequest request)
   {
      var records = await _recordsService.FetchAsync(request.Year, request.Month);
      records = records.OrderByDescending(r => r.Score).ToList();

      var persons = await _personService.FetchAsync();
      var views = new List<PersonRecordView>();
      foreach (var record in records)
      {
         record.Person = persons.FirstOrDefault(x => x.Id == record.PersonId);
         views.Add(record.MapViewModel(_mapper));
      }

      var items = views.MapReportItemList();
      string title = $"{request.Year}年{request.Month}月 書記官聽打成績";
      var model = new PersonRecordReportModel(title, request, items);
      var doc = new PersonRecordReportDocument(model);

      byte[] bytes = doc.GeneratePdf();
      return Ok(new BaseFileView(title, bytes));
   }

   Dictionary<string, string> ValidateFile(IFormFile file)
   {
      var errors = new Dictionary<string, string>();
      if (file == null)
      {
         errors.Add("file", "必須上傳檔案");
         return errors;
      }
      else
      {
         // Check file extension
         var allowedExtensions = new[] { ".csv" };
         var extension = Path.GetExtension(file.FileName).ToLower();

         if (!allowedExtensions.Contains(extension))
         {
            errors.Add("file", "只接受 csv 檔案");
            return errors;
         }

         return errors;
      }
   }


}