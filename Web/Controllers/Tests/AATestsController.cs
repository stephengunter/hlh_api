using Microsoft.AspNetCore.Mvc;
using ApplicationCore.DataAccess;
using Microsoft.SqlServer.Dac;
using Ardalis.Specification;
namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   public AATestsController(DefaultContext defaultContext)
   {
   }
   [HttpGet]
   public async Task<ActionResult> Index(string val)
   {
      return Ok(IsValid(val));
      //double increaseRate = ((double)(131 - 137) / 137) * 100;
      //return Ok(Math.Round(increaseRate, 2));
      //Math.Round(rate, 2).ToString();
      //return Ok(Math.Round(rate, 2, MidpointRounding.AwayFromZero).ToString());
   }

   bool IsValid(string val)
   {
      if (string.IsNullOrEmpty(val)) return false;
      if (val.Length != 6) return false;
      if (val.FirstOrDefault().ToString().ToUpper() != "U") return false;
      return true;
   }
   void ExportDatabaseToBacpac(string connectionString, string bacpacFilePath)
   {
      try
      {
         // Create an instance of DacServices with the connection string
         DacServices dacServices = new DacServices(connectionString);

         // Subscribe to the Message event to receive status messages
         dacServices.Message += (sender, e) => Console.WriteLine(e.Message);

         Console.WriteLine("Starting export...");

         // Perform the export
         dacServices.ExportBacpac(bacpacFilePath, "hlh_api");

         Console.WriteLine($"Export completed. Bacpac file saved to: {bacpacFilePath}");
      }
      catch (Exception ex)
      {
         Console.WriteLine($"An error occurred: {ex.Message}");
      }
   }
}
