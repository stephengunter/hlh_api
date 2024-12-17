using Microsoft.AspNetCore.Mvc;
using ApplicationCore.DataAccess;
using Microsoft.SqlServer.Dac;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Models;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Helpers;
using ApplicationCore.Services.Fetches;
using AutoMapper;
using System;
using ApplicationCore.Consts;
using Web.Models.IT;
using ApplicationCore.Models.IT;
using OfficeOpenXml;
using Infrastructure.Consts;
using OfficeOpenXml.Style;

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
   public static List<string> ProcessMultiLineColumn(string filePath, int columnNumber)
   {
      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var results = new List<string>();

      if (!System.IO.File.Exists(filePath))
      {
         throw new FileNotFoundException($"The file '{filePath}' was not found.");
      }

      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[0]; // Read the first worksheet

         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }

         int rowCount = worksheet.Dimension.Rows; // Total rows

         // Iterate through the rows of the specified column
         for (int row = 1; row <= rowCount; row++)
         {
            var cellValue = worksheet.Cells[row, columnNumber].Text;

            if (!string.IsNullOrEmpty(cellValue))
            {
               // Split content by newlines and insert a placeholder where necessary
               var lines = cellValue.Split(new[] { "\n", "\r" }, StringSplitOptions.None);
               for (int i = 0; i < lines.Length; i++)
               {
                  if (string.IsNullOrWhiteSpace(lines[i]))
                  {
                     lines[i] = "?"; // Insert a placeholder for empty lines
                  }
               }

               // Join the processed lines back with commas
               var singleLineValue = string.Join(",", lines).Trim();
               results.Add(singleLineValue);
            }
         }
      }

      return results;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      string filePath = @"C:\test\20241212\Filtered_DOGE.xlsx";
      string outputFilePath = @"C:\test\20241212\not_closed.xlsx";

      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      try
      {
         using (var package = new ExcelPackage(new FileInfo(filePath)))
         {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
               return BadRequest("No worksheet found in the Excel file.");

            using (var newPackage = new ExcelPackage())
            {
               var newWorksheet = newPackage.Workbook.Worksheets.Add("FilteredData");
               int newRow = 1;

               // Copy header row
               for (int col = 1; col <= worksheet.Dimension.Columns; col++)
               {
                  newWorksheet.Cells[newRow, col].Value = worksheet.Cells[1, col].Value;
               }
               newRow++;

               // Iterate through rows and filter by column B
               for (int row = 2; row <= worksheet.Dimension.Rows; row++)
               {
                  if (worksheet.Cells[row, 9].Text.Trim() != "¤wµ²®×")
                  {
                     for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                     {
                        newWorksheet.Cells[newRow, col].Value = worksheet.Cells[row, col].Value;
                     }
                     newRow++;
                  }
               }

               // Save the new Excel file
               await newPackage.SaveAsAsync(new FileInfo(outputFilePath));
            }
         }

         return Ok("Filtered rows have been written to the new Excel file.");
      }
      catch (Exception ex)
      {
         return StatusCode(500, $"An error occurred: {ex.Message}");
      }
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


   class PcData
   {
      public PcData(string first, string last)
      {
         First = first;
         Last = last;
      }

      public string First { get; set; }
      public string Last { get; set; }
   }
}
