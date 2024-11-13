using ApplicationCore.Services.Files;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using ApplicationCore.Consts;
using Microsoft.Extensions.Logging;
using Microsoft.Build.Logging;

namespace WinFormsApp_DbBackup;

public class App
{
   private readonly ILogger<App> _logger;
   private readonly List<DbSettings> _dbSettingsList;
   private readonly FileBackupSettings _backupSettings;
   private readonly IFileStoragesService _destinationFileService;
   private const string TEMP_FOLDER_PATH = @"C:\temp";
   public App(ILogger<App> logger, IOptions<FileBackupSettings> backupSettings, 
      IOptions<List<DbSettings>> dbSettingsOptions, FileStoragesServiceFactory fileStoragesServiceFactory)
   {
      _logger = logger;
      _dbSettingsList = dbSettingsOptions.Value;
      _backupSettings = backupSettings.Value;
      _destinationFileService = fileStoragesServiceFactory.Create(_backupSettings.Destination);
   }

   string TargetFolderPath => DateTime.Today.GetDateString();
   string FullBakFileName(DbSettings settings) => $"{settings.Name}_full_backup.bak";

   public void TestRun()
   {
      foreach (var dbSettings in _dbSettingsList)
      {

         _logger.LogInformation($"dbSettings {dbSettings.Name}:");
         if (dbSettings.FullBackupSettings != null)
         {
            _logger.LogInformation($"FullBackup StartAtTime {dbSettings.FullBackupSettings.GetStartAtTime()}:");
         }
      }
   }
   public async Task Run()
   {
      
      foreach (var dbSettings in _dbSettingsList)
      {
         
         string backupPath = CreateFullBackup(dbSettings!, TEMP_FOLDER_PATH);
         string copiedFilePath = "";
         // Open the source file as a stream
         using (var fileStream = new FileStream(backupPath, FileMode.Open, FileAccess.Read))
         {
            // Use the Create method to create a copy of the file
            copiedFilePath = _destinationFileService.Create(fileStream, TargetFolderPath, FullBakFileName(dbSettings));
            _logger.LogInformation($"File copied successfully to: {copiedFilePath}");
         }

         if (File.Exists(backupPath))
         {
            Thread.Sleep(1000);
            File.Delete(backupPath);
         }
      }
      
   }

   string CreateFullBackup(DbSettings dbSettings, string backupFolder)
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

      try
      {
         // Connection string setup
         string connectionString = $"Server={dbSettings.Host};Database={dbSettings.Name};User Id={dbSettings.Username};Password={dbSettings.Password};TrustServerCertificate=True;";

         // Add a row to TEST table

         // Execute the backup command
         using (var connection = new SqlConnection(connectionString))
         {
            connection.Open();
            if (dbSettings.CanTest) 
            {
               string query = "INSERT INTO TEST (Title) VALUES (@Title);";
               using (var command = new SqlCommand(query, connection))
               {
                  command.Parameters.AddWithValue("@Title", $"test row at {DateTime.Now.ToDateTimeString()}");

                  int rowsAffected = command.ExecuteNonQuery();
                  _logger.LogInformation($"test row at db:{dbSettings.Name} added. ");
               }

               Thread.Sleep(1000);
            }

            using (var command = new SqlCommand(backupQuery, connection))
            {
               command.ExecuteNonQuery();
            }
         }

         _logger.LogInformation($"db:{dbSettings.Name} full backup completed successfully.");
         return backupFilePath; // Return path to backup file
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "An error occurred while running the application logic.");
         throw; // Rethrow the exception to handle it elsewhere if needed
      }
   }

}
