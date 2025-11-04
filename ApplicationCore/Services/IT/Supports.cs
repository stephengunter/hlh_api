using ApplicationCore.Views.IT;
using Microsoft.Data.SqlClient;

namespace ApplicationCore.Services;

public interface IITServiceSupport
{
   IEnumerable<SupportViewModel> Fetch(int year, int month);
}


public class ITServiceSupport : IITServiceSupport
{
   private readonly string _connectionString;

   public ITServiceSupport(string connectionString)
	{
      _connectionString = connectionString;
   }
   public IEnumerable<SupportViewModel> Fetch(int year, int month)
   {
      var records = new List<SupportViewModel>();
      string query = @"
      SELECT * FROM [HLHWebDB2].[dbo].[questions]
      WHERE YEAR(CONVERT(date, q_date1, 111)) = @year
      AND MONTH(CONVERT(date, q_date1, 111)) = @month
      ORDER BY CONVERT(date, q_date1, 111)";

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
               var record = new SupportViewModel
               {
                  Department = reader["q_room"]?.ToString(),
                  User = reader["q_name"]?.ToString(),
                  Kind = reader["q_syskind"]?.ToString(),
                  Name = reader["q_sysname"]?.ToString(),
                  Content = reader["q_script"]?.ToString(),
                  Date = reader["q_date1"]?.ToString(),
                  Result = reader["q_pass"]?.ToString(),
                  Person = reader["mis"]?.ToString(),
                  PersonCount = reader["total"] != DBNull.Value ? Convert.ToInt32(reader["total"]) : 0,
               };

               records.Add(record);
            }
         }
      }
      return records;
   }

}
