using Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Services;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Reflection.PortableExecutable;
using System.Text;
using System.DirectoryServices;
using static QuestPDF.Helpers.Colors;
using HtmlAgilityPack;
using ApplicationCore.Models.Cars;
using ApplicationCore.Migrations;

namespace ConsoleDev;
class XProgram
{
   

   static int xxxxxxMain(string[] args)
   {
      
      return 0;
   }


   public static int xxMain(string[] args)
   {
      var hlh_props = DeviceHelpers.GetPropertiesFromFile(@"C:\Users\Administrator\Downloads\20250215_done\hlh_props.xlsx");

      string filePath = @"C:\Users\Administrator\Downloads\20250215_done\props.xlsx";
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var types = DeviceHelpers.GetPropTypes();
      types.Add(new PropType { Title = "伺服器", Code = "S" });
      types.Add(new PropType { Title = "網路交換器", Code = "W" });
      types.Add(new PropType { Title = "圖形掃描器", Code = "BS" });
      foreach (var type in types)
      {
         var list = DeviceHelpers.GetProperties(filePath, type);
         if (list.GroupBy(x => x.Num).Any(group => group.Count() > 1)) throw new Exception("Has Duplicate Num");
         list = list.OrderBy(x => x.GetNumber).ToList();

         foreach (var item in list)
         {
            var exist = hlh_props.FirstOrDefault(x => x.Num == item.Num);
            if (exist != null)
            {
               item.MinUse = exist.MinUse;
               item.Used = exist.Used;
               item.Over = exist.Over;
            }
         }

         string ouputPath = Path.Combine(@"C:\Users\Administrator\Downloads\20250215_done\outputs\props", $"{type.Code}_prop_done.xlsx");
         DeviceHelpers.OutputProps(list, ouputPath);
      }
      return 0;
   }



   static int Main_02145(string[] args)
   {
      string filePath = @"C:\Users\Administrator\Downloads\20250215\trash.xlsx";
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var types = DeviceHelpers.GetPropTypes();
      foreach (var type in types)
      {
         var list = DeviceHelpers.GetTrashProperties(filePath, type);
         if (list.GroupBy(x => x.Code).Any(group => group.Count() > 1)) throw new Exception("Has Duplicate Code");
         list = list.OrderBy(x => x.GetCodeNumber).ToList();



         string ouputPath = Path.Combine(@"C:\Users\Administrator\Downloads\20250215\outputs", $"{type.Code}_trash_done.xlsx");
         DeviceHelpers.GetOutput(list, ouputPath);
      }
      return 0;
   }
  
   static int ___Main(string[] args)
   {
      string filePath = @"C:\Users\Administrator\Downloads\20250214.xlsx";
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[0];

         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }
         var properties = new List<Property>();
         int rowCount = worksheet.Dimension.Rows; // Total rows
         for (int row = 2; row < rowCount; row++)
         {
            var prop = new Property()
            {
               Num = worksheet.Cells[row, 1].Text,
               Name = worksheet.Cells[row, 2].Text,
               Title = worksheet.Cells[row, 3].Text,
               Over = worksheet.Cells[row, 4].Text.Trim() == "是",
               Brand = worksheet.Cells[row, 6].Text,
               Type = worksheet.Cells[row, 5].Text,
               Unit = worksheet.Cells[row, 7].Text,
               Location = worksheet.Cells[row, 9].Text
            };

            properties.Add(prop);
         }

         var ids = new List<string>() { "621", "623", "801" };
         properties = properties.Where(x => x.Over).Where(x => ids.Contains(x.Location)).ToList();
         var grouped = properties.GroupBy(x => x.Location);

         var list = new List<Property>();
         foreach (var group in grouped)
         {
            foreach (var prop in group)
            {
               list.Add(prop);
            }
         }

         
        
         string outputPath = @"C:\Users\Administrator\Downloads\20250215\output.xlsx";
         DeviceHelpers.Output(list, outputPath);
         
         
         Console.ReadLine();
         //// Iterate through the rows of the specified column
         //for (int row = 2; row <= 100; row++)
         //{
         //   string strNum = worksheet.Cells[row, 2].Text;
         //   if (string.IsNullOrEmpty(strNum)) continue;
         //   list.Add(new Trash
         //   {
         //      Num = worksheet.Cells[row, 2].Text,
         //      Name = worksheet.Cells[row, 4].Text,
         //      Brand = worksheet.Cells[row, 5].Text,
         //      PS = worksheet.Cells[row, 6].Text
         //   });
         //}
      }
      return 0;
   }
   
   
   static int xxxxxMain(string[] args)
   {
      string filePath = @"C:\Users\Administrator\Downloads\20250211\done.xlsx";
      var list = DeviceHelpers.ReadTrashLcd(filePath);

      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("trash_order");

         // Add headers
         worksheet.Cells[1, 1].Value = "";
         worksheet.Cells[1, 2].Value = "設備編號";
         worksheet.Cells[1, 3].Value = "名稱";
         worksheet.Cells[1, 4].Value = "廠牌/型式";
         worksheet.Cells[1, 5].Value = "報廢單據";

         int row = 2;
         foreach (var item in list)
         {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = item.Num;
            worksheet.Cells[row, 3].Value = item.Name;
            worksheet.Cells[row, 4].Value = item.Brand;
            worksheet.Cells[row, 5].Value = item.PS;
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content

         string outputPath = @"C:\Users\Administrator\Downloads\20250211\output.xlsx";
         // Save the Excel package to a file
         using (var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{filePath}' created successfully!");
      }
      return 0;
   }

   
   static int xxxMain(string[] args)
   {
      string filePath = @"C:/temp/20250110/trash_order.xlsx";

      try
      {
         var trashlist = ReadTrash();
         using (var excelPackage = new ExcelPackage())
         {
            var worksheet = excelPackage.Workbook.Worksheets.Add("trash_order");

            // Add headers
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Email";

            int row = 2; // Start from the second row for data
            var grouped = trashlist.GroupBy(u => u.Name);
            foreach (var group in grouped)
            {
               foreach (var trash in group.OrderBy(u => u.GetNumber))
               {
                  worksheet.Cells[row, 1].Value = trash.Num;
                  worksheet.Cells[row, 2].Value = trash.P_Num;
                  worksheet.Cells[row, 3].Value = trash.Name;
                  worksheet.Cells[row, 4].Value = trash.Brand;
                  worksheet.Cells[row, 5].Value = trash.PS;
                  row++;
               }
               
            }

            // Apply some basic formatting
            worksheet.Cells[1, 1, 1, 3].Style.Font.Bold = true; // Make headers bold
            worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content

            // Save the Excel package to a file
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
               excelPackage.SaveAs(stream);
            }

            Console.WriteLine($"Excel file '{filePath}' created successfully!");
         }
      }
      catch (Exception ex)
      {
         Console.WriteLine($"Error: {ex.Message}");
      }
      
      return 0;
   }

   static List<Trash> ReadTrash()
   {
      string filePath = @"C:/temp/20250110/trash.xlsx";

      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      var records = new List<Trash>();

      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var workbook = package.Workbook;
         var worksheet = workbook.Worksheets[0]; // 

         // Add headers
         var rowCount = worksheet.Dimension.Rows;
         var colCount = worksheet.Dimension.Columns;

         for (int row = 2; row <= rowCount; row++) // Start from row 2 to skip headers
         {
            var trash = new Trash
            {
               Num = worksheet.Cells[row, 1].Text,
               P_Num = worksheet.Cells[row, 2].Text,                // Column B: Name
               Name = worksheet.Cells[row, 3].Text,                 // Column B: Name
               Brand = worksheet.Cells[row, 4].Text,                // Column B: Name
               PS = worksheet.Cells[row, 5].Text,                // Column C: Email
            };

            records.Add(trash);
         }
      }
      return records;
   }
   static int MovePC()
   {
      string filePath = @"C:/temp/1227/舊電腦_已入庫_1226.csv";
      var records = ReadPcCsvFile(filePath);
      records.Sort();

      // Define your SQL Server connection string
      string connectionString = "Server=localhost;Database=HLHWebDB2;Trusted_Connection=True;TrustServerCertificate=True;";
      try
      {
         int rows = 0;
         string num = "";

         string ps = "";
         // Connect to the SQL Server
         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            for (int i = 0; i < records.Count; i++)
            {
               int id = records[i];
               num = $"PP{id}";
               string selectQuery = "SELECT [memo_a] FROM [hlh_device] WHERE [de_no] = @no";

               using (SqlCommand command = new SqlCommand(selectQuery, connection))
               {
                  // Add parameter to the query
                  command.Parameters.AddWithValue("@no", num); // Replace with your desired `de_no` value

                  // Execute the query and read the result
                  object result = command.ExecuteScalar();

                  string a = "2024/12月 更換新電腦";
                  string b = "2024/12/24 收回資訊室庫房";
                  string memoA = "";
                  if (result != null)
                  {
                     memoA = result.ToString();
                     ps = $"{memoA}\n";
                  }
                  ps = ps + $"{a}\n" + $"{b}";
               }
               string updateQuery = "UPDATE [hlh_device] SET [username] = @NewUser, [state] = @NewState, "
                   + "[room] = @NewRoom, [work_m] = @NewWork, [memo_a] = @Ps "
                  + "WHERE [de_no] = @no";
               using (SqlCommand command = new SqlCommand(updateQuery, connection))
               {
                  // Add parameters to avoid SQL injection
                  command.Parameters.AddWithValue("@NewUser", ""); // Replace with your value
                  command.Parameters.AddWithValue("@NewState", "庫存(勘用品)"); // Replace with your value
                  command.Parameters.AddWithValue("@NewRoom", "資訊室庫房"); // Replace with your value
                  command.Parameters.AddWithValue("@NewWork", ""); // Replace with your value
                  command.Parameters.AddWithValue("@Ps", ps); // Replace with your value

                  command.Parameters.AddWithValue("@no", num);

                  // Execute the command
                  int rowsAffected = command.ExecuteNonQuery();
                  if (rowsAffected > 0) rows++;
               }
            }


         }
        
         Console.WriteLine($"{rows} row(s) updated.");
         Console.ReadLine();
         return 0; // Success
      }
      catch (Exception ex)
      {
         Console.WriteLine($"An error occurred: {ex.Message}");
         return 1; // Failure
      }
      return 0;
   }
   static int MoveLcd(string[] args)
   {
      string filePath = @"C:/temp/1224/lcd.csv";
      var records = ReadLcdCsvFile(filePath);
      records.Sort();

      // Define your SQL Server connection string
      string connectionString = "Server=172.17.129.51;Database=HLHWebDB2;User Id=sa;Password=hlh2022$$;TrustServerCertificate=True";

      try
      {
         int rows = 0;
         string num = "";
       
         string ps = "";
         // Connect to the SQL Server
         using (SqlConnection connection = new SqlConnection(connectionString))
         {
            connection.Open();
            for (int i = 0; i < records.Count; i++) 
            {
               int id = records[i];
               num = $"L{id}";
               string selectQuery = "SELECT [memo_a] FROM [hlh_device] WHERE [de_no] = @no";

               using (SqlCommand command = new SqlCommand(selectQuery, connection))
               {
                  // Add parameter to the query
                  command.Parameters.AddWithValue("@no", num); // Replace with your desired `de_no` value

                  // Execute the query and read the result
                  object result = command.ExecuteScalar();

                  string a = "2024/12月 更換新電腦";
                  string b = "2024/12/24 收回資訊室庫房";
                  string memoA = "";
                  if (result != null)
                  {
                     memoA = result.ToString();
                     ps = $"{memoA}\n";
                  }
                  ps = ps + $"{a}\n" + $"{b}";  
               }
               string updateQuery = "UPDATE [hlh_device] SET [username] = @NewUser, [state] = @NewState, "
                   + "[room] = @NewRoom, [work_m] = @NewWork, [memo_a] = @Ps "
                  + "WHERE [de_no] = @no";
               using (SqlCommand command = new SqlCommand(updateQuery, connection))
               {
                  // Add parameters to avoid SQL injection
                  command.Parameters.AddWithValue("@NewUser", ""); // Replace with your value
                  command.Parameters.AddWithValue("@NewState", "庫存(勘用品)"); // Replace with your value
                  command.Parameters.AddWithValue("@NewRoom", "資訊室庫房"); // Replace with your value
                  command.Parameters.AddWithValue("@NewWork", ""); // Replace with your value
                  command.Parameters.AddWithValue("@Ps", ps); // Replace with your value

                  command.Parameters.AddWithValue("@no", num);

                  // Execute the command
                  int rowsAffected = command.ExecuteNonQuery();
                  if(rowsAffected > 0) rows++;
               }
            }
            

         }
         Console.WriteLine($"{rows} row(s) updated.");
         Console.ReadLine();
         return 0; // Success
      }
      catch (Exception ex)
      {
         Console.WriteLine($"An error occurred: {ex.Message}");
         return 1; // Failure
      }
      return 0;
   }

   static void Read()
   {
      string filePath = @"C:/temp/1224/lcd.csv";
      string outputFilePath = @"C:/temp/1224/lcd_recult.xlsx";

      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      var records = ReadLcdCsvFile(filePath);
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
   static List<int> ReadPcCsvFile(string filePath)
   {
      var records = new List<int>();
      using (var reader = new StreamReader(filePath, Encoding.UTF8))
      {
         while (!reader.EndOfStream)
         {
            var line = reader.ReadLine();

            var values = line!.Split(',');
            if (values.Length == 2) records.Add(int.Parse(values[1]));
         }
      }

      return records;
   }
   static List<int> ReadLcdCsvFile(string filePath)
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
}

