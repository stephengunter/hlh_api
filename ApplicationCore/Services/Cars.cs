using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;

namespace ApplicationCore.Services;

public class CarUser
{ 
   public int Id { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Department { get; set; } = string.Empty;
   public string AD { get; set; } = string.Empty;

   public bool IsValid()
   {
      return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(AD);
   }
   
}

public class CarRecord
{
   public int Id { get; set; }
   public int UserId { get; set; }
   public string Name { get; set; } = string.Empty;
   public string Department { get; set; } = string.Empty;
}
public class CarsService
{
   //string _connectionString = "Server=localhost;Database=HLHWebDB1;Trusted_Connection=True;TrustServerCertificate=True;";
   string _connectionString = "Server=172.17.129.51;Database=HLHWebDB1;User Id=sa;Password=hlh2022$$;TrustServerCertificate=True;";
   public List<CarUser> ReadUsersFromFile(string filePath)
   {
      var users = new List<CarUser>();
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
         var worksheet = package.Workbook.Worksheets.FirstOrDefault();
         for (int row = 2; row <= worksheet!.Dimension.Rows; row++)
         {
            if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text.Trim())) break;
            users.Add(new CarUser
            {
               AD = worksheet.Cells[row, 1].Text.Trim(),
               Name = worksheet.Cells[row, 2].Text.Trim(),
               Department = worksheet.Cells[row, 3].Text.Trim()
            });

         }
      }
      return users;
   }
   public List<CarUser> GetCarUsers()
   {
      var carUsers = new List<CarUser>();
      using (var conn = new SqlConnection(_connectionString))
      {
         conn.Open();
         string query = "SELECT [id], [Name], [Department], [AD] FROM [HLHWebDB1].[dbo].[CarUsers]";
         using (var cmd = new SqlCommand(query, conn))
         {
            using (var reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  carUsers.Add(new CarUser
                  {
                     Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                     Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                     AD = reader["AD"] != DBNull.Value ? reader["AD"].ToString() : string.Empty
                  });
               }
            }
         }
      }
      return carUsers;
   }
   public int InsertCarUser(CarUser user)
   {
      using (var conn = new SqlConnection(_connectionString))
      {
         conn.Open();
         string query = "INSERT INTO [HLHWebDB1].[dbo].[CarUsers] ([Name], [Department], [AD]) VALUES (@Name, @Department, @AD); SELECT SCOPE_IDENTITY();";
         using (var cmd = new SqlCommand(query, conn))
         {
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Department", user.Department);
            cmd.Parameters.AddWithValue("@AD", user.AD);
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
         }
      }
   }
   public CarUser GetCarUserByAD(string ad)
   {
      CarUser? user = null;
      using (var conn = new SqlConnection(_connectionString))
      {
         conn.Open();
         string query = "SELECT [id], [Name], [Department], [AD] FROM [HLHWebDB1].[dbo].[CarUsers] WHERE [AD] = @AD";
         using (var cmd = new SqlCommand(query, conn))
         {
            cmd.Parameters.AddWithValue("@AD", ad);
            using (var reader = cmd.ExecuteReader())
            {
               if (reader.Read())
               {
                  user = new CarUser
                  {
                     Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                     Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                     Department = reader["Department"] != DBNull.Value ? reader["Department"].ToString() : string.Empty,
                     AD = reader["AD"] != DBNull.Value ? reader["AD"].ToString() : string.Empty
                  };
               }
            }
         }
      }
      return user!;
   }
   public CarUser FindCarUser(int id)
   {
      CarUser? user = null;
      using (var conn = new SqlConnection(_connectionString))
      {
         conn.Open();
         string query = "SELECT [id], [Name], [Department], [AD] FROM [HLHWebDB1].[dbo].[CarUsers] WHERE [id] = @id";
         using (var cmd = new SqlCommand(query, conn))
         {
            cmd.Parameters.AddWithValue("@id", id);
            using (var reader = cmd.ExecuteReader())
            {
               if (reader.Read())
               {
                  user = new CarUser
                  {
                     Id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
                     Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty,
                     Department = reader["Department"] != DBNull.Value ? reader["Department"].ToString() : string.Empty,
                     AD = reader["AD"] != DBNull.Value ? reader["AD"].ToString() : string.Empty
                  };
               }
            }
         }
      }
      return user!;
   }
   public void UpdateCarUser(CarUser user)
   {
      using (var conn = new SqlConnection(_connectionString))
      {
         conn.Open();
         string query = "UPDATE [HLHWebDB1].[dbo].[CarUsers] SET [Name] = @Name, [Department] = @Department, [AD] = @AD WHERE [id] = @id";
         using (var cmd = new SqlCommand(query, conn))
         {
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Department", user.Department);
            cmd.Parameters.AddWithValue("@AD", user.AD);
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.ExecuteNonQuery();
         }
      }
   } 
   public void DeleteCarUser(int id)
   {
      using (var conn = new SqlConnection(_connectionString))
      {
         conn.Open();
         string query = "DELETE FROM [HLHWebDB1].[dbo].[CarUsers] WHERE [id] = @id";
         using (var cmd = new SqlCommand(query, conn))
         {
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
         }
      }
   }

   public void UpdateCarRecord(CarRecord record)
   {
      using (var conn = new SqlConnection(_connectionString))
      {
         conn.Open();
         string query = "UPDATE [HLHWebDB1].[dbo].[Car] SET [UserId] = @UserId, [Name] = @Name WHERE [Applyid] = @id";
         using (var cmd = new SqlCommand(query, conn))
         {
            cmd.Parameters.AddWithValue("@UserId", record.UserId);
            cmd.Parameters.AddWithValue("@Name", record.Name);
            cmd.Parameters.AddWithValue("@id", record.Id);
            cmd.ExecuteNonQuery();
         }
      }
   }
   public List<CarRecord> GetCarRecords()
   {
      var carRecords = new List<CarRecord>();
      using (var conn = new SqlConnection(_connectionString))
      {
         conn.Open();
         string query = "SELECT [Applyid], [UserId], [Name] FROM [HLHWebDB1].[dbo].[Car]";
         using (var cmd = new SqlCommand(query, conn))
         {
            using (var reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  int id = reader["Applyid"] != DBNull.Value ? Convert.ToInt32(reader["Applyid"]) : 0;
                  int userId = 0;
                  if (reader["UserId"] != DBNull.Value && int.TryParse(reader["UserId"].ToString(), out var parsed))
                  {
                     userId = parsed;
                  }
                  string name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty;
                  carRecords.Add(new CarRecord
                  {
                     Id = id,
                     Name = name,
                     UserId = userId
                  });
               }
            }
         }
      }
      return carRecords;
   }
   //public List<CarRecord> FetchCarRecordsByName(string name)
   //{
   //   var carRecords = new List<CarRecord>();
   //   using (var conn = new SqlConnection(_connectionString))
   //   {
   //      conn.Open();
   //      string query = "SELECT [Applyid], [UserId], [Name] FROM [HLHWebDB1].[dbo].[Car] WHERE [Name] = @name";
   //      using (var cmd = new SqlCommand(query, conn))
   //      {
   //         cmd.Parameters.AddWithValue("@name", name);
   //         using (var reader = cmd.ExecuteReader())
   //         {
   //            while (reader.Read())
   //            {
   //               int id = reader["Applyid"] != DBNull.Value ? Convert.ToInt32(reader["Applyid"]) : 0;
   //               int userId = 0;
   //               if (reader["UserId"] != DBNull.Value && int.TryParse(reader["UserId"].ToString(), out var parsed))
   //               {
   //                  userId = parsed;
   //               }
   //               string name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty;
   //               carRecords.Add(new CarRecord
   //               {
   //                  Id = id,
   //                  Name = name,
   //                  UserId = userId
   //               });
   //            }
   //         }
   //      }
   //   }
   //   return carRecords;
   //}

}
