using ApplicationCore.Views.IT;
using Microsoft.Data.SqlClient;

namespace ApplicationCore.Services;

public interface IServiceFixes
{
   IEnumerable<FixViewModel> Fetch(int year, int month);
}
public class ServiceHlh01Fixes : IServiceFixes
{
   private readonly string _connectionString;

   public ServiceHlh01Fixes(string connectionString)
	{
      _connectionString = connectionString;

   }
   public IEnumerable<FixViewModel> Fetch(int year, int month)
   {
      var records = new List<FixViewModel>();
      string query = @"
      SELECT * FROM [HLHWebDB2].[dbo].[devfix_m]
      WHERE YEAR(CONVERT(date, date1, 111)) = @year
      AND MONTH(CONVERT(date, date1, 111)) = @month
      ORDER BY CONVERT(date, date1, 111)";

      using (var conn = new SqlConnection(_connectionString))
      using (var cmd = new SqlCommand(query, conn))
      {
         cmd.Parameters.AddWithValue("@year", year);
         cmd.Parameters.AddWithValue("@month", month);

         conn.Open();
         using (var reader = cmd.ExecuteReader())
         {
            while (reader.Read())
            {
               var record = new FixViewModel
               {
                  User = reader["user1"]?.ToString(),
                  Number = reader["fixno"]?.ToString(),
                  Name = reader["dev_name"]?.ToString(),
                  Content = reader["spc"]?.ToString(),
                  Date = reader["date1"]?.ToString(),
                  Ps = reader["fix_s"]?.ToString(),
                  Result = reader["memo"]?.ToString(),
                  Count = reader["qua"] != DBNull.Value ? Convert.ToInt32(reader["qua"]) : 0
               };

               records.Add(record);
            }
         }
      }
      return records;
   }

}
