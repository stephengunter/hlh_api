using ApplicationCore.Consts;
using ApplicationCore.Services.Files;
using ApplicationCore.Settings;
using Infrastructure.Consts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;

namespace WinFormsApp_DbBackup;

public class Startup
{
   public IConfiguration Configuration { get; private set; }
   public Startup()
   {
      var environment = Environment.GetEnvironmentVariable(EnvironmentKeys.ASPNETCORE_ENVIRONMENT);
      // Load appsettings.json
      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile($"appsettings.{environment}.json")
          .Build();

      this.Configuration = configuration;
   }

   public ServiceCollection ConfigureServices()
   {
      var services = new ServiceCollection();
      services.Configure<FileBackupSettings>(Configuration.GetSection(SettingsKeys.FileBackup));
      services.Configure<List<DbSettings>>(Configuration.GetSection("DbSettings"));

      services.AddLogging(configure => configure.AddSerilog()); // Add Serilog
      services.AddSingleton<FileStoragesServiceFactory>();
      services.AddTransient<App>();

      services.AddWindowsFormsBlazorWebView();

      return services;
   }
}
