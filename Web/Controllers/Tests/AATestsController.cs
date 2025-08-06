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
using Microsoft.Data.SqlClient;
using ApplicationCore.Models.Cars;
using ApplicationCore.Helpers;
using ApplicationCore.Services.Keyin;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ApplicationCore.Views;
using System.Collections.Generic;

namespace Web.Controllers.Tests;

public class TestU
{ 
   public string Name { get; set; }
   public string PID { get; set; }
}

public class AATestsController : BaseTestController
{
   private readonly IUsersService _usersService;
   private readonly DefaultContext _defaultContext;
   private readonly List<DbSettings> _dbSettingsList;
   
   private readonly IDepartmentsService _departmentsService;
   private readonly IMapper _mapper;
   public AATestsController(IOptions<List<DbSettings>> dbSettingsOptions, DefaultContext defaultContext,
      IUsersService usersService, IDepartmentsService departmentsService, IMapper mapper)
   {
      _dbSettingsList = dbSettingsOptions?.Value ?? new List<DbSettings>();
      _defaultContext = defaultContext;
      _usersService = usersService;
      _departmentsService = departmentsService;
      _mapper = mapper;
   }

   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var users = await _usersService.FetchAllAsync();
      return Ok(users);
   }

   List<TestU> GetTUS()
   {
      var tus = new List<TestU>();
      string connectionString = "Server=172.17.128.99;Database=HLH;User Id=sa;Password=hlh2000$$;TrustServerCertificate=True;";
      using (var conn = new SqlConnection(connectionString))
      {
         conn.Open();
         string query = "SELECT DISTINCT [perid], [pername] FROM [HLH].[dbo].[readlist]";
         using (var cmd = new SqlCommand(query, conn))
         {
            using (var reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  tus.Add(new TestU { PID = reader["perid"].ToString(), Name = reader["pername"].ToString() });
               }
            }
         }
      }
      return tus;
   }

  
   async Task TUS()
   {
      string connectionString = "Server=172.17.128.99;Database=HLH;User Id=sa;Password=hlh2000$$;TrustServerCertificate=True;";
      var tus = GetTUS();
      var users = await _usersService.FetchAllAsync();
      var dpts = await _departmentsService.FetchAllAsync();
      var views = new List<UserViewModel>();
      foreach (var dpt in dpts)
      {
         using (var conn = new SqlConnection(connectionString))
         {
            conn.Open();
            var dpt_uers = users.Where(x => x.Profiles != null && x.Profiles.DepartmentId.HasValue && x.Profiles.DepartmentId.Value == dpt.Id);
            foreach (var user in dpt_uers)
            {
               var tu = tus.FirstOrDefault(x => x.Name == user.FullName);
               if (tu == null) continue;
               string query = "SELECT COUNT(*) FROM HLH_View WHERE perid = @perid";
               using (var cmd = new SqlCommand(query, conn))
               {
                  cmd.Parameters.AddWithValue("@perid", tu.PID);

                  int count = (int)cmd.ExecuteScalar();
                  if (count < 1) continue;
                  string insertQuery = "UPDATE HLH_View SET passwd = @ad WHERE perid = @perid";

                  using (var insertCmd = new SqlCommand(insertQuery, conn))
                  {
                     insertCmd.Parameters.AddWithValue("@ad", user.Name);
                     insertCmd.Parameters.AddWithValue("@perid", tu.PID);

                     int rowsAffected = insertCmd.ExecuteNonQuery();
                     Console.WriteLine($"DEBUG: {rowsAffected} row(s) inserted.");
                  }
               }

            }
         }
         
         
      }
      
   }
   
  
   static List<string> ProcessMultiLineColumn(string filePath, int columnNumber)
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

   List<LocalApplyCar> GetLocalUser()
   {
      var local = new List<LocalApplyCar>();
      string connectionString = "Server=localhost;Database=HLHWebDB1;Trusted_Connection=True;TrustServerCertificate=True;";
      string query = "SELECT * FROM ApplyCar";
      using (var conn = new SqlConnection(connectionString))
      {
         try
         {
            conn.Open();
            using (var cmd = new SqlCommand(query, conn))
            {
               using (var reader = cmd.ExecuteReader())
               {
                  while (reader.Read())
                  {
                     var record = new LocalApplyCar
                     {
                        Applyid = reader.IsDBNull(0) ? "" : reader.GetInt32(0).ToString(), // Column Index 0 for Id
                        CarNO = reader.IsDBNull(1) ? "" : reader.GetString(1), // Column Index 1 for Name
                        CarType = reader.IsDBNull(2) ? "" : reader.GetString(2), // Column Index 2 for Department.

                        Id = reader.IsDBNull(3) ? "" : reader.GetString(3), // Column Index 0 for Id
                        Name = reader.IsDBNull(4) ? "" : reader.GetString(4), // Column Index 1 for Name
                        dept = reader.IsDBNull(5) ? "" : reader.GetString(5), // Column Index 2 for Department

                        Sdate = reader.IsDBNull(6) ? "" : reader.GetString(6), // Column Index 0 for Id
                        Edate = reader.IsDBNull(7) ? "" : reader.GetString(7), // Column Index 1 for Name
                        STime = reader.IsDBNull(8) ? "" : reader.GetString(8), // Column Index 2 for Department

                        Etime = reader.IsDBNull(9) ? "" : reader.GetString(9), // Column Index 0 for Id
                        Object = reader.IsDBNull(10) ? "" : reader.GetString(10), // Column Index 1 for Name
                        content = reader.IsDBNull(11) ? "" : reader.GetString(11), // Column Index 2 for Department

                        toName = reader.IsDBNull(12) ? "" : reader.GetString(12), // Column Index 0 for Id
                        dpt = reader.IsDBNull(13) ? "" : reader.GetString(13), // Column Index 1 for Name
                        Preport = reader.IsDBNull(14) ? "" : reader.GetString(14), // Column Index 2 for Department

                        mark = reader.IsDBNull(15) ? "" : reader.GetString(15), // Column Index 0 for Id
                        driver = reader.IsDBNull(16) ? "" : reader.GetString(16), // Column Index 1 for Name
                        driver2 = reader.IsDBNull(17) ? "" : reader.GetString(17), // Column Index 2 for Department


                        CheckName = reader.IsDBNull(18) ? "" : reader.GetString(18), // Column Index 0 for Id
                        CheckDateTime = reader.IsDBNull(19) ? "" : reader.GetString(19),// Column Index 1 for Name
                        Checkstate = reader.IsDBNull(20) ? "" : reader.GetString(20), // Column Index 2 for Department

                        UpdateUser = reader.IsDBNull(21) ? "" : reader.GetString(21), // Column Index 0 for Id
                        Updatedatetime = reader.IsDBNull(22) ? "" : reader.GetDateTime(22).ToString("yyyy-MM-dd HH:mm:ss"), // Column Index 1 for Name
                     };
                     local.Add(record);
                  }
               }
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine($"Error: {ex.Message}");
         }
      }

      return local;
   }
   
   void SaveRemote(List<LocalApplyCar> cars)
   {
      string connectionString = "Server=172.17.129.51;User ID=sa;Password=hlh2022$$;Database=HLHWebDB1;TrustServerCertificate=True;"; // Replace with your connection string
      string query = @"
            INSERT INTO ApplyCar
            (Applyid, CarNO, CarType, UserId, Name, dept, Sdate, Edate, STime, Etime, Object, content, toName, dpt, Preport, mark, driver, driver2, CheckName, CheckDateTime, Checkstate, UpdateUser, Updatedatetime) 
            VALUES 
            (@Applyid, @CarNO, @CarType, @UserId, @Name, @dept, @Sdate, @Edate, @STime, @Etime, @Object, @content, @toName, @dpt, @Preport, @mark, @driver, @driver2, @CheckName, @CheckDateTime, @Checkstate, @UpdateUser, @Updatedatetime)";

      using (SqlConnection conn = new SqlConnection(connectionString))
      {
         try
         {
            conn.Open();
            // Enable IDENTITY_INSERT
            using (SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT ApplyCar ON", conn))
            {
               cmd.ExecuteNonQuery();
            }

            foreach (var car in cars)
            {
               using (SqlCommand cmd = new SqlCommand(query, conn))
               {
                  // Add parameters for each property
                  cmd.Parameters.AddWithValue("@Applyid", car.Applyid ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@CarNO", car.CarNO ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@CarType", car.CarType ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@UserId", car.Id ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@Name", car.Name ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@dept", car.dept ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@Sdate", car.Sdate ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@Edate", car.Edate ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@STime", car.STime ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@Etime", car.Etime ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@Object", car.Object ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@content", car.content ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@toName", car.toName ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@dpt", car.dpt ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@Preport", car.Preport ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@mark", car.mark ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@driver", car.driver ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@driver2", car.driver2 ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@CheckName", car.CheckName ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@CheckDateTime", car.CheckDateTime ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@Checkstate", car.Checkstate ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@UpdateUser", car.UpdateUser ?? (object)DBNull.Value);
                  cmd.Parameters.AddWithValue("@Updatedatetime", car.Updatedatetime ?? (object)DBNull.Value);

                  // Execute the query
                  cmd.ExecuteNonQuery();
               }
            }
            // Disable IDENTITY_INSERT
            using (SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT ApplyCar OFF", conn))
            {
               cmd.ExecuteNonQuery();
            }
            Console.WriteLine("All rows inserted successfully.");
         }
         catch (Exception ex)
         {
            Console.WriteLine($"An error occurred: {ex.Message}");
         }
      }
   }
   async Task Test()
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
               return;

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

         return;
      }
      catch (Exception ex)
      {
         return;
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

public class LocalApplyCar
{
   public string Applyid { get; set; }
   public string CarNO { get; set; }
   public string CarType { get; set; }
   public string Id { get; set; }
   public string Name { get; set; }
   public string dept { get; set; }
   public string Sdate { get; set; }
   public string Edate { get; set; }
   public string STime { get; set; }
   public string Etime { get; set; }
   public string Object { get; set; }
   public string content { get; set; }
   public string toName { get; set; }
   public string dpt { get; set; }
   public string Preport { get; set; }
   public string mark { get; set; }
   public string driver { get; set; }
   public string driver2 { get; set; }
   public string CheckName { get; set; }
   public string CheckDateTime { get; set; }
   public string Checkstate { get; set; }
   public string UpdateUser { get; set; }
   public string Updatedatetime { get; set; }
}

