using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddTransient<App>();
      // 由這邊切換一般Ping Test或Htpp Request Test
      //serviceCollection.AddTransient<IPingTestService, PintTestByHttpRequestService>();
      return serviceCollection;
   }
}
