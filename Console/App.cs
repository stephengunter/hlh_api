using ApplicationCore.Services.Files;
using ApplicationCore.Settings;
using Autofac.Features.Metadata;
using Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using System;

namespace ConsoleDev;

public class App
{
   private readonly IFileStoragesService _sourceFileService;
   private readonly IFileStoragesService _destinationFileService;
   private readonly DbSettings _dbSettings;
   public App(IOptions<DbSettings> dbSettings)
   {
      _dbSettings = dbSettings.Value;
      //_sourceFileService = fileStoragesServiceFactory.Create(_backupSettings.Source);
      //_destinationFileService = fileStoragesServiceFactory.Create(_backupSettings.Destination);
   }
   string ConnectionString => $"Server={_dbSettings.Host};Database={_dbSettings.Name};User Id={_dbSettings.Username};Password={_dbSettings.Password};TrustServerCertificate=True;";


   public async Task Run()
   {
      return;
      try
      {
         string? sourceFilePath = null;

         while (true)
         {
            Console.WriteLine("Please input Excel file path (.xls or .xlsx):");
            sourceFilePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
               Console.WriteLine("❌ Input is empty. Please try again.");
               continue;
            }

            if (!File.Exists(sourceFilePath))
            {
               Console.WriteLine("❌ File not found. Please try again.");
               continue;
            }

            string extension = Path.GetExtension(sourceFilePath).ToLower();
            if (extension != ".xls" && extension != ".xlsx")
            {
               Console.WriteLine("❌ This is not an Excel file. Please try again.");
               continue;
            }

            // 合法檔案，跳出迴圈
            break;
         }

         var properties = GetDevicesFromFile(sourceFilePath!);
         foreach (var property in properties)
         {
            property.Ps = GetDeviceData(property);

            UpdateDevice(property);
         }

         Console.WriteLine("✅ File processing completed.");
      }
      catch (Exception ex)
      {
         LogInfo($"An error occurred: {ex.Message}");
      }

   }

   List<Property> GetDevicesFromFile(string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      var properties = new List<Property>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[0];
         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }
         int rowCount = worksheet.Dimension.Rows; // Total rows
         for (int row = 2; row <= rowCount; row++)
         {
            if (int.TryParse(worksheet.Cells[row, 1].Text.Trim(), out int orderNumber))
            {
               string num = worksheet.Cells[row, 2].Text.Trim();
               string ps = worksheet.Cells[row, 5].Text.Trim();
               var property = new Property()
               {
                  Code = num,
                  Ps = ps
               };
               properties.Add(property);
            }
         }
      }
      return properties;
   }

   string GetDeviceData(Property property)
   {
      string ps = "";
      using (SqlConnection connection = new SqlConnection(ConnectionString))
      {
         connection.Open();
         string selectQuery = "SELECT [no1], [memo_a] FROM [hlh_device] WHERE [de_no] = @no";
         using (SqlCommand command = new SqlCommand(selectQuery, connection))
         {
            // Add parameter to the query
            command.Parameters.AddWithValue("@no", property.Code);

            // Execute the query and read the results, should be only one row


            //if no match result, throw exception with property.Code
            using (SqlDataReader reader = command.ExecuteReader())
            {
               if (!reader.HasRows)
               {
                  // ❌ 找不到資料，拋出例外
                  throw new Exception($"No record found for de_no = {property.Code}");
               }

               while (reader.Read())
               {
                  // ✅ 找到資料，可讀取欄位
                  string no1 = reader["no1"].ToString();
                  ps = reader["memo_a"] is DBNull ? "" : reader["memo_a"].ToString();

                  string msg = property.Ps;
                  if (string.IsNullOrEmpty(ps)) ps = msg;
                  else ps = ps + $"\n" + $"{msg} , 114/8/1清出";  
               }
            }
         }
      }
      return ps;
   }

   void UpdateDevice(Property property)
   {
      using (SqlConnection connection = new SqlConnection(ConnectionString))
      {
         connection.Open();
         string updateQuery = "UPDATE [hlh_device] SET " + "[fired] = @fired, [quit] = @quit, [date_o] = @date_o," +
            "[username] = @NewUser, [state] = @NewState, "
                   + "[room] = @NewRoom, [work_m] = @NewWork, [memo_a] = @Ps "
                  + "WHERE [de_no] = @no";
         using (SqlCommand command = new SqlCommand(updateQuery, connection))
         {
            // Add parameters to avoid SQL injection
            command.Parameters.AddWithValue("@fired", 1); // Replace with your value
            command.Parameters.AddWithValue("@quit", 1); // Replace with your value
            command.Parameters.AddWithValue("@date_o", "2025/8/1"); // Replace with your value
            command.Parameters.AddWithValue("@NewUser", ""); // Replace with your value
            command.Parameters.AddWithValue("@NewState", "已報廢處理"); // Replace with your value
            command.Parameters.AddWithValue("@NewRoom", ""); // Replace with your value
            command.Parameters.AddWithValue("@NewWork", ""); // Replace with your value
            command.Parameters.AddWithValue("@Ps", property.Ps); // Replace with your value

            command.Parameters.AddWithValue("@no", property.Code);

            // Execute the command
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
               throw new Exception($"Update failed: No record found for code {property.Code}");
            }
         }
      }
   }


   void LogInfo(string msg = "")
   {
      if (String.IsNullOrEmpty(msg))
      {
         Console.WriteLine("backup success.");
      }
      else
      {
         Console.WriteLine(msg);
      }
      Console.ReadLine();
   }
}
