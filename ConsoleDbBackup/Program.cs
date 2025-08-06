using System;
using Serilog;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConsoleDbBackup;

class Program
{
   public static async Task Main(string[] args)
   {
      Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // Set the minimum log level
                .WriteTo.Console()          // Log to console
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // Log to file with daily rotation
                .CreateLogger();

      try
      {
        
         string arg = "full";
         if (args.HasItems() && args[0].Equals("diff")) arg = "diff";
         Log.Information("arg: " + arg);

        

         Log.Information("Application is starting...");

         var startUp = new Startup();
         var serviceProvider = startUp.ConfigureServices().BuildServiceProvider();
         var app = serviceProvider.GetRequiredService<App>();
         //if (arg == "diff") await app.RunDiff();
         //else await app.RunFull();
         app.TestRun();




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

