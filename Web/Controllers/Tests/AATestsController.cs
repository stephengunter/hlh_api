using ApplicationCore.Models;
using ApplicationCore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Infrastructure.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Services;
using QuestPDF.Helpers;
using System.IO;
using MiniExcelLibs;
using Microsoft.EntityFrameworkCore;
namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly DefaultContext _defaultContext;
   public AATestsController(DefaultContext defaultContext)
   {
      _defaultContext = defaultContext;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      string path = @"C:\temp\hlh22.csv";
      var records = ReadCsvFile(path);
      foreach (var record in records)
      {
         var ets = record.Person;
         Console.WriteLine(ets);
         await AddDocModelIfNotExist(_defaultContext, record);
      }
      _defaultContext.SaveChanges();
      return Ok();
   }

   List<DocModel> ReadCsvFile(string path)
   {
     
      var records = new List<DocModel>();
      //using (var reader = new StreamReader(path, Encoding.UTF8))
      using (var reader = new StreamReader(path))
      {
         // Skip the header line
         var header = reader.ReadLine();
         while (!reader.EndOfStream)
         {
            var line = reader.ReadLine();
            var values = line!.Split(',');

            var record = new DocModel
            {
               Num = values[0].Trim(),
               Old_Num = values[1].Trim(),
               Old_CNum = values[2].Trim(),
               Unit = values[3].Trim(),
               Date = values[4].Trim(),
               Person = values[5].Trim(),
               Result = values[6].Trim(),
               Title = values[7].Trim()
            };

            records.Add(record);
         }
      }
      return records;
   }
   async Task AddDocModelIfNotExist(DefaultContext context, DocModel record)
   {
      var exist = await context.DocModels.FirstOrDefaultAsync(x => x.Num == record.Num);
      if (exist == null) context.DocModels.Add(record);
      else
      {
         record.SetValuesTo(exist);
         context.DocModels.Update(exist);
      }
   }
   void ExportDatabaseToBacpac(string connectionString, string bacpacFilePath)
   {
      try
      {
         // Create an instance of DacServices with the connection string
         DacServices dacServices = new DacServices(connectionString);

         // Subscribe to the Message event to receive status messages
         dacServices.Message += (sender, e) => Console.WriteLine(e.Message);

         Console.WriteLine("Starting export...");

         // Perform the export
         dacServices.ExportBacpac(bacpacFilePath, "hlh_api");

         Console.WriteLine($"Export completed. Bacpac file saved to: {bacpacFilePath}");
      }
      catch (Exception ex)
      {
         Console.WriteLine($"An error occurred: {ex.Message}");
      }
   }
}
