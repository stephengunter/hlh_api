using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System.Text;
using static QuestPDF.Helpers.Colors;

namespace ConsoleDev;
class Program
{
   static int Main(string[] args)
   {
      return 0;
   }
   static int MovePC()
   {
      string filePath = @"C:/temp/1227/舊電腦_已入庫_1226.csv";
      var records = ReadPcCsvFile(filePath);
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


public class LCD
{
   public string Num { get; set; } = string.Empty;
   public string Ps { get; set; } = string.Empty;
}
