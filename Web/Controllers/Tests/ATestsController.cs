using ApplicationCore.DataAccess;
using ApplicationCore.Migrations;
using ApplicationCore.Models;
using Ardalis.Specification;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly DefaultContext _defaultContext;
   public ATestsController(DefaultContext defaultContext)
   {
      _defaultContext = defaultContext;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      string filePath = @"C:/temp/1224/lcd.csv";
      string outputFilePath = @"C:/temp/1224/lcd_recult.xlsx";

      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      var records = ReadCsvFile(filePath);
      records.Sort();

      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("lcds");

         // Add headers
         worksheet.Cells[1, 1].Value = "ID";
         worksheet.Cells[1, 2].Value = "Name";
         worksheet.Cells[1, 3].Value = "Email";

         // Add some user data
         var lcds = new List<LCD>();
         for (int i = 0; i < records.Count; i++) 
         {
            lcds.Add(new LCD { Num = $"L{records[i]}" });
         }

         int row = 2; // Start from the second row for data
         foreach (var lcd in lcds)
         {
            worksheet.Cells[row, 1].Value = lcd.Num;
            worksheet.Cells[row, 2].Value = "test";
            worksheet.Cells[row, 3].Value = "test";
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 3].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content

         // Save the Excel package to a file
         using (var stream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{outputFilePath}' created successfully!");
      }


      return Ok();
   }

   //async Task ReadLcd()
   //{
   //   string filePath = @"C:/temp/1224/lcd.csv";
   //   var records = ReadCsvFile(filePath);
   //   foreach (var record in records)
   //   {
   //      record.Tel = record.Tel.Replace('_', ',');
   //      await AddTelNameIfNotExist(_defaultContext, record);
   //   }
   //   _defaultContext.SaveChanges();
   //}

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
         record.SetValuesTo(exist);
         context.TelNames.Update(exist);
      }
   }

   List<int> ReadCsvFile(string filePath)
   {
      var records = new List<int>();
      using (var reader = new StreamReader(filePath, Encoding.UTF8))
      {
         while (!reader.EndOfStream)
         {
            var line = reader.ReadLine();
            
            var values = line!.Split(',');

            for (int i = 0; i < values.Length; i++)
            {
               if (values[i].Trim() == "Y")
               {
                  if (i > 0 && int.TryParse(values[i - 1].Trim(), out int number)) // Check the previous cell
                  {
                     records.Add(number); // Add the number to the list
                  }
               }
            }
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

public class LCD
{
   public string Num { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
}
