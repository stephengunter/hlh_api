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

namespace ConsoleDev;
class Program
{
   
   static int Main(string[] args)
   {
      var hlh_props = GetPropertiesFromFile(@"C:\Users\Administrator\Downloads\20250215_done\hlh_props.xlsx");

      string filePath = @"C:\Users\Administrator\Downloads\20250215_done\props.xlsx";
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var types = GetPropTypes();
      types.Add(new PropType { Title = "伺服器", Code = "S" });
      types.Add(new PropType { Title = "網路交換器", Code = "W" });
      types.Add(new PropType { Title = "圖形掃描器", Code = "BS" });
      foreach (var type in types)
      {
         var list = GetProperties(filePath, type);
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
         OutputProps(list, ouputPath);
      }
      return 0;
   }

   static List<Property> GetPropertiesFromFile(string filePath)
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
            var prop = new Property()
            {
               Num = worksheet.Cells[row, 1].Text.Trim(),
               MinUse = worksheet.Cells[row, 4].Text,
               Used = worksheet.Cells[row, 5].Text,
               Over = worksheet.Cells[row, 6].Text.Trim() == "是"
            };
            properties.Add(prop);
         }
      }
      return properties;
   }
   static List<Property> GetProperties(string filePath, PropType type)
   {
      var properties = new List<Property>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[type.Code];
         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }
         int rowCount = worksheet.Dimension.Rows; // Total rows
         for (int row = 2; row <= rowCount; row++)
         {
            var prop = new Property()
            {
               Code = worksheet.Cells[row, 2].Text,
               Num = worksheet.Cells[row, 3].Text.Trim(),
               Name = worksheet.Cells[row, 4].Text,
               Brand = worksheet.Cells[row, 5].Text
            };
            properties.Add(prop);
         }
      }
      return properties;
   }
   static void OutputProps(List<Property> properties, string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("sheet1");

         // Add headers
         worksheet.Cells[1, 1].Value = "";
         worksheet.Cells[1, 2].Value = "設備編號";
         worksheet.Cells[1, 3].Value = "財產編號";
         worksheet.Cells[1, 4].Value = "名稱";
         worksheet.Cells[1, 5].Value = "廠牌/型式";
         worksheet.Cells[1, 6].Value = "最低使用年限";
         worksheet.Cells[1, 7].Value = "已使用年月";
         worksheet.Cells[1, 8].Value = "廠牌已達年限";

         int row = 2;
         foreach (var item in properties)
         {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = item.Code;
            worksheet.Cells[row, 3].Value = item.Num;
            worksheet.Cells[row, 4].Value = item.Name;
            worksheet.Cells[row, 5].Value = item.Brand;
            worksheet.Cells[row, 6].Value = item.MinUse;
            worksheet.Cells[row, 7].Value = item.Used;
            worksheet.Cells[row, 8].Value = item.Over ? "是" : "";
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content


         // Save the Excel package to a file
         using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{filePath}' created successfully!");
      }
   }
   static int Main_02145(string[] args)
   {
      string filePath = @"C:\Users\Administrator\Downloads\20250215\trash.xlsx";
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var types = GetPropTypes();
      foreach (var type in types)
      {
         var list = GetTrashProperties(filePath, type);
         if (list.GroupBy(x => x.Code).Any(group => group.Count() > 1)) throw new Exception("Has Duplicate Code");
         list = list.OrderBy(x => x.GetCodeNumber).ToList();



         string ouputPath = Path.Combine(@"C:\Users\Administrator\Downloads\20250215\outputs", $"{type.Code}_trash_done.xlsx");
         GetOutput(list, ouputPath);
      }
      return 0;
   }
   static List<Property> GetTrashProperties(string filePath, PropType type)
   {
      var properties = new List<Property>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets[type.Code];
         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }
         int rowCount = worksheet.Dimension.Rows; // Total rows
         for (int row = 2; row <= rowCount; row++)
         {
            var prop = new Property()
            {
               Code = worksheet.Cells[row, 2].Text,
               Name = worksheet.Cells[row, 4].Text,
               Brand = worksheet.Cells[row, 5].Text,
               Ps = worksheet.Cells[row, 6].Text
            };
            properties.Add(prop);
         }
      }
      return properties;
   }
   static void GetOutput(List<Property> properties, string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("sheet1");

         // Add headers
         worksheet.Cells[1, 1].Value = "";
         worksheet.Cells[1, 2].Value = "設備編號";
         worksheet.Cells[1, 3].Value = "名稱";
         worksheet.Cells[1, 4].Value = "廠牌/型式";
         worksheet.Cells[1, 5].Value = "報廢單據";

         int row = 2;
         foreach (var item in properties)
         {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = item.Code;
            worksheet.Cells[row, 3].Value = item.Name;
            worksheet.Cells[row, 4].Value = item.Brand;
            worksheet.Cells[row, 5].Value = item.Ps;
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content


         // Save the Excel package to a file
         using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{filePath}' created successfully!");
      }
   }
   static List<PropType> GetPropTypes()
   {
      return new List<PropType>()
      {
         new PropType { Title = "印表機", Code = "TT" },
         new PropType { Title = "液晶螢幕", Code = "L" },
         new PropType { Title = "個人電腦", Code = "PP" },
         new PropType { Title = "不斷電系統", Code = "UP" },
         new PropType { Title = "其他", Code = "Other" }
      };
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
         Output(list, outputPath);
         
         
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
   static string UnitText(string code)
   {
      if (string.IsNullOrEmpty(code)) return "";
      code = code.Trim();
      if (code == "621") return "資訊室";
      if (code == "623") return "資訊室庫房";
      if (code == "801") return "電腦教室";
      return "";
   }
   static void Output(List<Property> properties, string filePath)
   {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage())
      {
         var worksheet = excelPackage.Workbook.Worksheets.Add("sheet1");

         // Add headers
         worksheet.Cells[1, 1].Value = "";
         worksheet.Cells[1, 2].Value = "財產編號";
         worksheet.Cells[1, 3].Value = "名稱";
         worksheet.Cells[1, 4].Value = "別名";
         worksheet.Cells[1, 5].Value = "已達年限";
         worksheet.Cells[1, 6].Value = "廠牌";
         worksheet.Cells[1, 7].Value = "型式";
         worksheet.Cells[1, 8].Value = "保管單位";
         worksheet.Cells[1, 9].Value ="存置地點";

         int row = 2;
         foreach (var item in properties)
         {
            worksheet.Cells[row, 1].Value = row - 1;
            worksheet.Cells[row, 2].Value = item.Num;
            worksheet.Cells[row, 3].Value = item.Name;
            worksheet.Cells[row, 4].Value = item.Title;
            worksheet.Cells[row, 5].Value = item.Over ? "是" : "";
            worksheet.Cells[row, 6].Value = item.Brand;
            worksheet.Cells[row, 7].Value = item.Type;
            worksheet.Cells[row, 8].Value = "資訊室";
            worksheet.Cells[row, 9].Value = UnitText(item.Location) ;
            row++;
         }

         // Apply some basic formatting
         worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true; // Make headers bold
         worksheet.Cells.AutoFitColumns(); // Adjust column width to fit content

         
         // Save the Excel package to a file
         using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
         {
            excelPackage.SaveAs(stream);
         }

         Console.WriteLine($"Excel file '{filePath}' created successfully!");
      }
   }
   static int xxxxxMain(string[] args)
   {
      string filePath = @"C:\Users\Administrator\Downloads\20250211\done.xlsx";
      var list = ReadTrashLcd(filePath);

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

   static List<Trash> ReadTrashLcd(string filePath)
   {
      // Enable EPPlus license context for non-commercial use
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var list = new List<Trash>();
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets["0210"];

         if (worksheet == null)
         {
            throw new InvalidOperationException("No worksheet found in the Excel file.");
         }

         int rowCount = worksheet.Dimension.Rows; // Total rows

         // Iterate through the rows of the specified column
         for (int row = 2; row <= 100; row++)
         {
            string strNum = worksheet.Cells[row, 2].Text;
            if (string.IsNullOrEmpty(strNum)) continue;
            list.Add(new Trash
            {
               Num = worksheet.Cells[row, 2].Text,
               Name = worksheet.Cells[row, 4].Text,
               Brand = worksheet.Cells[row, 5].Text,
               PS = worksheet.Cells[row, 6].Text
            });
         }
      }
      if (list.GroupBy(x => x.Num).Any(group => group.Count() > 1)) throw new Exception("Has Duplicate Nums");
      return list.OrderBy(x => x.GetNumber).ToList();
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

