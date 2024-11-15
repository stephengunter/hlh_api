using ApplicationCore.Services.Files;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using ApplicationCore.Consts;
using Microsoft.Extensions.Logging;
using Microsoft.Build.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text;

namespace ConsoleDbBackup;

public class App
{
   private readonly ILogger<App> _logger;
   private readonly List<DbSettings> _dbSettingsList;
   private readonly FileBackupSettings _backupSettings;
   private readonly IFileStoragesService _destinationFileService;
   private const string BACKUP_FOLDER_PATH = @"D:\db_backups";
   public App(ILogger<App> logger, IOptions<FileBackupSettings> backupSettings, 
      IOptions<List<DbSettings>> dbSettingsOptions, FileStoragesServiceFactory fileStoragesServiceFactory)
   {
      _logger = logger;
      _dbSettingsList = dbSettingsOptions.Value;
      _backupSettings = backupSettings.Value;
      _destinationFileService = fileStoragesServiceFactory.Create(_backupSettings.Destination);

      
   }

   string GetBakupFolder(string dbName) => Path.Combine(@"D:\db_backups", DateTime.Today.GetDateString(), dbName);
   string FullBakFileName(string dbName) => $"{dbName}_full_backup.bak";
   string DiffBakFileName(string dbName) => $"{dbName}_diff_backup.bak";
   string GetConnectionString(DbSettings dbSettings)
      => $"Server={dbSettings.Host};Database={dbSettings.Name};User Id={dbSettings.Username};Password={dbSettings.Password};TrustServerCertificate=True;";

   public async Task TestRun()
   {
      foreach (var dbSettings in _dbSettingsList)
      {
         string sql = SqlScriptGenerator.GenerateRestoreScript(dbSettings.Name, FullBakFileName(dbSettings.Name), DiffBakFileName(dbSettings.Name));
         File.WriteAllText(@"D:\db_backups\test.sql", sql);
      }
   }
   public async Task RunFull()
   {
      foreach (var dbSettings in _dbSettingsList)
      {
         string backupPath = CreateFullBackup(dbSettings);
         //string copiedFilePath = "";
         // Open the source file as a stream
         //using (var fileStream = new FileStream(backupPath, FileMode.Open, FileAccess.Read))
         //{
         //   // Use the Create method to create a copy of the file
         //   copiedFilePath = _destinationFileService.Create(fileStream, TargetFolderPath, FullBakFileName(dbSettings));
         //   _logger.LogInformation($"File copied successfully to: {copiedFilePath}");
         //}

         //if (File.Exists(backupPath))
         //{
         //   Thread.Sleep(1000);
         //   File.Delete(backupPath);
         //}
      }
   }
   public async Task RunDiff()
   {
      foreach (var dbSettings in _dbSettingsList)
      {
         string backupPath = CreateDiffBackup(dbSettings);
      }
   }

   void CheckProvider(DbSettings dbSettings)
   {
      if (!dbSettings.Provider.EqualTo(DbProvider.SQLServer))
      {
         throw new NotSupportedException("Only SQL Server is supported for this backup function.");
      }
   }

   string CreateFullBackup(DbSettings dbSettings)
   {
      CheckProvider(dbSettings);
      // Construct the backup file path and name
      string dbName = dbSettings.Name;
      string bakupFolder = GetBakupFolder(dbName);
      Directory.CreateDirectory(bakupFolder);

      string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
      string backupFileName = $"{dbName}_FullBackup_{timestamp}.bak";
      string backupFilePath = Path.Combine(bakupFolder, backupFileName);
      
      try
      {
         string connectionString = GetConnectionString(dbSettings); 
         using (var connection = new SqlConnection(connectionString))
         {
            connection.Open();
            if (dbSettings.CanTest) 
            {
               AddTestRow(connection, dbSettings);
            }

            // SQL command to create a full database backup
            string backupQuery = $"BACKUP DATABASE [{dbName}] TO DISK = '{backupFilePath}' WITH FORMAT, INIT;";

            using (var command = new SqlCommand(backupQuery, connection))
            {
               command.ExecuteNonQuery();
            }
         }

         _logger.LogInformation($"db:{dbName} full backup completed successfully.");
         return backupFilePath; // Return path to backup file
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "An error occurred while CreateFullBackup.");
         throw; // Rethrow the exception to handle it elsewhere if needed
      }
   }

   void AddTestRow(SqlConnection connection, DbSettings dbSettings)
   {
      string query = "INSERT INTO TEST (Title) VALUES (@Title);";
      using (var command = new SqlCommand(query, connection))
      {
         command.Parameters.AddWithValue("@Title", $"test row at {DateTime.Now.ToDateTimeString()}");

         int rowsAffected = command.ExecuteNonQuery();
         _logger.LogInformation($"test row at db:{dbSettings.Name} added.");
      }

      Thread.Sleep(1000);
   }
   string CreateDiffBackup(DbSettings dbSettings)
   {
      CheckProvider(dbSettings);
      // Construct the backup file path and name
      string dbName = dbSettings.Name;
      string bakupFolder = GetBakupFolder(dbName);
      Directory.CreateDirectory(bakupFolder);

      string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
      string backupFileName = $"{dbName}_DiffBackup_{timestamp}.bak";
      string backupFilePath = Path.Combine(bakupFolder, backupFileName);
      try
      {
         // Connection string setup
         string connectionString = GetConnectionString(dbSettings);
         using (var connection = new SqlConnection(connectionString))
         {
            connection.Open();
            if (dbSettings.CanTest)
            {
               AddTestRow(connection, dbSettings);
            }

            // SQL command to create a full database backup
            string backupQuery = $"BACKUP DATABASE [{dbName}] TO DISK = '{backupFilePath}' WITH DIFFERENTIAL;";

            using (var command = new SqlCommand(backupQuery, connection))
            {
               command.ExecuteNonQuery();
            }
         }

         _logger.LogInformation($"db:{dbName} diff backup completed successfully.");
         return backupFilePath; // Return path to backup file
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "An error occurred while CreateDiffBackup.");
         throw; // Rethrow the exception to handle it elsewhere if needed
      }
   }

   public string GenerateRestoreScript(string databaseName, string fullBackupPath, string diffBackupPath)
   {
      // Start building the SQL script using StringBuilder
      StringBuilder sqlScript = new StringBuilder();

      // Set the database to single-user mode with ROLLBACK IMMEDIATE
      sqlScript.AppendLine("USE master;");
      sqlScript.AppendLine();
      sqlScript.AppendLine($"-- Step 1: Set the database {databaseName} to single-user mode with ROLLBACK IMMEDIATE");
      sqlScript.AppendLine($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
      sqlScript.AppendLine();

      // Restore the full backup with NORECOVERY to prepare for differential restore
      sqlScript.AppendLine($"-- Step 2: Restore the full backup with NORECOVERY");
      sqlScript.AppendLine($"RESTORE DATABASE [{databaseName}]");
      sqlScript.AppendLine($"FROM DISK = '{fullBackupPath}'");
      sqlScript.AppendLine("WITH NORECOVERY, REPLACE;");
      sqlScript.AppendLine();

      // Restore the differential backup with RECOVERY
      sqlScript.AppendLine($"-- Step 3: Restore the differential backup with RECOVERY");
      sqlScript.AppendLine($"RESTORE DATABASE [{databaseName}]");
      sqlScript.AppendLine($"FROM DISK = '{diffBackupPath}'");
      sqlScript.AppendLine("WITH RECOVERY, REPLACE;");
      sqlScript.AppendLine();

      // Set the database back to multi-user mode
      sqlScript.AppendLine($"-- Step 4: Set the database {databaseName} back to multi-user mode");
      sqlScript.AppendLine($"ALTER DATABASE [{databaseName}] SET MULTI_USER;");

      return sqlScript.ToString();
   }

   public void SaveSqlScriptToFile(string sqlScript, string filePath)
   {
      // Write the SQL script to the specified file
      File.WriteAllText(filePath, sqlScript);
      Console.WriteLine($"SQL script has been written to {filePath}");
   }
}
