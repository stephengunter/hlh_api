using ApplicationCore.Models;
using ApplicationCore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Infrastructure.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Dac;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Services;
using QuestPDF.Helpers;
namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly ICryptoService _cryptoService;
   public AATestsController(ICryptoService cryptoService)
   {
      _cryptoService = cryptoService;
     
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      //var strList = new List<string>();
      //var lens = new List<int>() { 6, 8, 10, 12, 16, 20, 24, 28, 32, 36, 40, 48, 60, 64, 80, 85, 91, 96 };
      //for (int i = 0; i < 100; i++)
      //{
      //   strList.Add(RandomHelper.GenerateRandomString(lens.GetRandomItem()));
      //}
      //foreach (var str in strList) 
      //{ 
      //   var enc = _cryptoService.Encrypt(str);
      //   var dec = _cryptoService.Decrypt(enc);
      //   if (dec.Equals(str)) Console.WriteLine($"{enc} , {dec}");
      //   else throw new Exception("not equal");
      //}
      return Ok();
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
