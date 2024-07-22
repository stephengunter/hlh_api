using ApplicationCore.DataAccess;
using ApplicationCore.Migrations;
using ApplicationCore.Models;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly DefaultContext _defaultContext;
   public ATestsController(DefaultContext defaultContext)
   {
      _defaultContext = defaultContext;
   }
   [HttpGet("locations")]
   public async Task<ActionResult> Locations()
   {
      string filePath = @"C:/temp/tels.csv";
      var records = ReadCsvFile(filePath);
      foreach (var record in records) 
      {
         record.Tel = record.Tel.Replace('_', ',');
         await AddTelNameIfNotExist(_defaultContext, record);
      }
      return Ok(records);
   }

   async Task AddTelNameIfNotExist(DefaultContext context, TelName record)
   {
      if (context.TelNames.Count() == 0)
      {
         context.TelNames.Add(record);
         return;
      }
      var exist = await context.TelNames.FirstOrDefaultAsync(x => x.Name == record.Name);
      if (exist == null) context.TelNames.Add(record);
      else 
      {
         // update existing entity
         context.TelNames.Update(exist);
      }
   }

   List<TelName> ReadCsvFile(string filePath)
   {
      var records = new List<TelName>();
      using (var reader = new StreamReader(filePath, Encoding.UTF8))
      {
         // Skip the header line
         var header = reader.ReadLine();

         while (!reader.EndOfStream)
         {
            var line = reader.ReadLine();
            var values = line!.Split(',');

            var record = new TelName
            {
               Department = values[0].Trim(),
               Role = values[1].Trim(),
               Name = values[2].Trim(),
               Ad =  values[3].Trim(),
               Tel = values[4].Trim()
            };

            records.Add(record);
         }
      }

      return records;
   }



   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
