using Infrastructure.Consts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace WinFormsApp_DbBackup
{
   internal static class Program
   {
      
      /// <summary>
      ///  The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // Set the minimum log level
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // Log to file with daily rotation
                .CreateLogger();
         try
         {
            Log.Information("Application is starting...");

            var configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json")
             .Build();

            var environment = configuration[$"Environment:{EnvironmentKeys.ASPNETCORE_ENVIRONMENT}"];
            if (string.IsNullOrEmpty(environment))
            {
               Environment.SetEnvironmentVariable(EnvironmentKeys.ASPNETCORE_ENVIRONMENT, EnvironmentNames.Development);
            }
            else if (environment == EnvironmentNames.Production)
            {
               Environment.SetEnvironmentVariable(EnvironmentKeys.ASPNETCORE_ENVIRONMENT, EnvironmentNames.Production);
            }
            else 
            {
               Environment.SetEnvironmentVariable(EnvironmentKeys.ASPNETCORE_ENVIRONMENT, EnvironmentNames.Development);
            }
            var startUp = new Startup();
            var serviceProvider = startUp.ConfigureServices().BuildServiceProvider();

            // Start application
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //var app = serviceProvider.GetRequiredService<App>();
            Application.Run(new Form1(serviceProvider));

            Log.Information("Application has completed successfully.");
         }
         catch (Exception ex)
         {
            Log.Fatal(ex, "Application terminated unexpectedly");
         }
         finally
         {
            Log.CloseAndFlush(); // Ensure all logs are flushed before exit
         }
         
      }
   }
}