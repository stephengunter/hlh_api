using ApplicationCore.Consts;
using ApplicationCore.Settings;
using Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ApplicationCore.Services.Dbs;

public interface IDbBackupService
{
   string CreateFullBackup(DbSettings dbSettings, string backupFolder);
}
public class SQLServerDbBackupService : IDbBackupService
{
   public string CreateFullBackup(DbSettings dbSettings, string backupFolder)
   {
      // Ensure the provider is SQL Server
      if (!dbSettings.Provider.EqualTo(DbProvider.SQLServer))
      {
         throw new NotSupportedException("Only SQL Server is supported for this backup function.");
      }

      // Construct the backup file path and name
      string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
      string backupFileName = $"{dbSettings.Name}_FullBackup_{timestamp}.bak";
      string backupFilePath = Path.Combine(backupFolder, backupFileName);

      // Ensure the backup folder exists
      Directory.CreateDirectory(backupFolder);

      // SQL command to create a full database backup
      string backupQuery = $"BACKUP DATABASE [{dbSettings.Name}] TO DISK = '{backupFilePath}' WITH FORMAT, INIT;";

      // Connection string setup
      string connectionString = $"Server={dbSettings.Host};Database={dbSettings.Name};User Id={dbSettings.Username};Password={dbSettings.Password};TrustServerCertificate=True;";

      // Add a row to TEST table

      // Execute the backup command
      using (var connection = new SqlConnection(connectionString))
      {
         connection.Open();
         using (var command = new SqlCommand(backupQuery, connection))
         {
            command.ExecuteNonQuery();
         }
      }

      return backupFilePath; // Return path to backup file
   }
}