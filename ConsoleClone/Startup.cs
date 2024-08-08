using ApplicationCore.Consts;
using ApplicationCore.Services.Files;
using ApplicationCore.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleClone;

public class Startup
{
   public IConfiguration Configuration { get; private set; }
   public Startup()
   {
      // 預設為appsettings.json
      var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: false);
      // 讀取環境變數
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

      // 有環境變數設定則將appsettings.{environment}.json才是我們要建置的
      if (!string.IsNullOrEmpty(environment))
      {
         builder = builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);
      }
      // 最後建置並指派給Configuration變數
      this.Configuration = builder.Build();
   }

   public ServiceCollection ConfigureServices()
   {
      var services = new ServiceCollection();
      services.Configure<FileBackupSettings>(Configuration.GetSection(SettingsKeys.FileBackup));
      services.AddSingleton<FileStoragesServiceFactory>();
      services.AddTransient<App>();
      services.AddTransient<Test>();


      return services;
   }
}
