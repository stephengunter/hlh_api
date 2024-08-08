using System;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleClone;

class Program
{
   public static async Task Main(string[] args)
   {
      var startUp = new Startup();
      var serviceProvider = startUp.ConfigureServices().BuildServiceProvider();
      var app = serviceProvider.GetRequiredService<App>();
      await app.Run();
   }
}

