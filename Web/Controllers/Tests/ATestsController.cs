using ApplicationCore.DataAccess;
using ApplicationCore.Migrations;
using ApplicationCore.Models;
using Ardalis.Specification;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.IO.Packaging;
using System.Text;
using static OfficeOpenXml.ExcelErrorValue;

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
      string filePath = @"C:\\Users\\Administrator\\Downloads\\1140210\\allall.csv";
      var records = new List<HLHProp>();
      string outputFilePath = @"C:/temp/1140210/all.xlsx";

      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("sheet1");
         int i = 1; 
         using (var reader = new StreamReader(filePath, Encoding.UTF8))
         {
            while (!reader.EndOfStream)
            {
               var line = reader.ReadLine();
               if (i == 1)
               {
                  worksheet.Cells[1, 1].Value = "財編";
                  worksheet.Cells[1, 2].Value = "名稱";
                  worksheet.Cells[1, 3].Value = "存置地點";
                  worksheet.Cells[1, 4].Value = "已達年限";
                  worksheet.Cells[1, 5].Value = "已減損";

                  i++;
               }
               else
               {
                  var values = line!.Split('\t');
                  if (values[4].Trim() == "623")
                  {
                     worksheet.Cells[i, 1].Value = values[1];
                     worksheet.Cells[i, 2].Value = values[2];
                     worksheet.Cells[i, 3].Value = "資訊室庫房";
                     worksheet.Cells[i, 4].Value = values[5];
                     worksheet.Cells[i, 5].Value = values[6];

                     i++;
                  }
               }
            }
         }

         // Save the Excel package to a file
         using (var stream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }
         
      }
      return Ok();
   }
   async Task<ActionResult> xxxxxIndex()
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      string filePath = @"C:/temp/20250206/new_pc.xlsx";
      var list = new List<PcExchange>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[0]; // Read the first worksheet

         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }
         int rowCount = worksheet.Dimension.Rows; // Total rows

         //3.保管人 4.PP634 , 5.新主機財編   6.新螢幕  7.新螢幕財編  8.舊主機  9.舊主機財編
         // Iterate through the rows of the specified column
         int lcd = 641;
         for (int row = 2; row <= rowCount; row++)
         {
            var cellValue = worksheet.Cells[row, 3].Text;

            list.Add(new PcExchange
            {
               Pos = worksheet.Cells[row, 2].Text,
               Person = worksheet.Cells[row, 3].Text,
               NewPP = worksheet.Cells[row, 4].Text,
               NewPropNum = worksheet.Cells[row, 5].Text,
               NewLCD = worksheet.Cells[row, 6].Text,
               NewLCDPropNum = worksheet.Cells[row, 7].Text,
               OldPP = worksheet.Cells[row, 8].Text,
               OldPropNum = worksheet.Cells[row, 9].Text,
            });
            lcd++;
         }

        
      }

      
      await UpdatePropNum(list);
      //await WriteExcel(list, @"C:/temp/20250206/new_pc.xlsx");
      return Ok(list);
   }
   async Task UpdatePropNum(List<PcExchange> list)
   {
      string connectionString = "Server=172.17.129.51;Database=HLHWebDB2;User Id=sa;Password=hlh2022$$;TrustServerCertificate=True";

      //string connectionString = "Server=localhost;Database=HLHWebDB2;Trusted_Connection=True;TrustServerCertificate=True;";
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
         connection.Open();
         foreach (var item in list)
         {
            string updateQuery = "UPDATE [hlh_device] SET [de_hlhno] = @NewHLHno"
                  + " WHERE [de_no] = @no";
            using (SqlCommand command = new SqlCommand(updateQuery, connection))
            {
               // Add parameters to avoid SQL injection
               command.Parameters.AddWithValue("@NewHLHno", item.NewPropNum);
               command.Parameters.AddWithValue("@no", item.NewPP);
               // Execute the command
               int rowsAffected = command.ExecuteNonQuery();
            }
         }
      }
   }
   async Task WriteExcel(List<PcExchange> list, string filePath)
   {
      using (var package = new ExcelPackage())
      {
         var sheet = package.Workbook.Worksheets.Add("pc");
         sheet.Cells[1, 2].Value = "存置地點";
         sheet.Cells[1, 3].Value = "保管人";
         sheet.Cells[1, 4].Value = "新主機";
         sheet.Cells[1, 5].Value = "新主機財編";
         sheet.Cells[1, 6].Value = "新螢幕";
         sheet.Cells[1, 7].Value = "新螢幕財編";
         sheet.Cells[1, 8].Value = "舊主機";
         sheet.Cells[1, 9].Value = "舊主機財編";
         for (int i = 0; i < list.Count; i++)
         { 
            var item = list[i];
            sheet.Cells[i + 2, 2].Value = item.Pos;
            sheet.Cells[i + 2, 3].Value = item.Person;
            sheet.Cells[i + 2, 4].Value = item.NewPP;
            sheet.Cells[i + 2, 5].Value = item.NewPropNum;
            sheet.Cells[i + 2, 6].Value = item.NewLCD;
            sheet.Cells[i + 2, 7].Value = item.NewLCDPropNum;
            sheet.Cells[i + 2, 8].Value = item.OldPP;
            sheet.Cells[i + 2, 9].Value = item.OldPropNum;

         }
         await package.SaveAsAsync(new FileInfo(filePath));
      }
   }

   async Task ReadLcd()
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
public class PcExchange
{
   public string Pos { get; set; } = string.Empty;
   public string Person { get; set; } = string.Empty;
   public string NewPP { get; set; } = string.Empty;
   public string NewPropNum { get; set; } = string.Empty;
   public string NewLCD { get; set; } = string.Empty;
   public string NewLCDPropNum { get; set; } = string.Empty;
   public string OldPP { get; set; } = string.Empty;
   public string OldPropNum { get; set; } = string.Empty;
}

public class HLHProp
{
   public string Pos { get; set; } = string.Empty;
   public string Person { get; set; } = string.Empty;
   public string IT_Num { get; set; } = string.Empty;
   public string PropNum { get; set; } = string.Empty;
   public string Name { get; set; } = string.Empty;
   public string OutLimit { get; set; } = string.Empty;
   public string Active { get; set; } = string.Empty;
}
