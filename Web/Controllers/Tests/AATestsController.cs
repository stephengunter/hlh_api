using Microsoft.AspNetCore.Mvc;
using ApplicationCore.DataAccess;
using Microsoft.SqlServer.Dac;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Helpers;
using ApplicationCore.Services.Fetches;
using AutoMapper;
using System;

namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly UserManager<User> _userManager;
   private readonly DefaultContext _defaultContext;
   private readonly List<DbSettings> _dbSettingsList;
   public AATestsController(IOptions<List<DbSettings>> dbSettingsOptions, DefaultContext defaultContext, UserManager<User> userManager)
   {
      _dbSettingsList = dbSettingsOptions?.Value ?? new List<DbSettings>();
      _defaultContext = defaultContext;
      _userManager = userManager;
   }

   [HttpGet]
   public async Task<ActionResult> Index()
   {
      
      return Ok();
   }
   List<Department> ReadDepartmentsFromCsv(string filePath)
   {
      var records = new List<Department>();
      using (var reader = new StreamReader(filePath, Encoding.UTF8))
      {
         // Skip the header line
         var header = reader.ReadLine();
         while (!reader.EndOfStream)
         {
            var line = reader.ReadLine();
            var values = line!.Split(',');
            int id = int.Parse(values[0]);
            string title = values[2];
            string pid = values[3];
            if (string.IsNullOrEmpty(title)) continue;

            int? parentId = string.IsNullOrEmpty(pid) ? null : int.Parse(pid);   


            var exist = records.FirstOrDefault(x => x.Title == title);
            if (exist == null)
            {
               records.Add(new Department { Id = id, Title = title, ParentId = parentId });
            }
         }
      }

      return records;
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