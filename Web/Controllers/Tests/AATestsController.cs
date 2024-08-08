using ApplicationCore.Models;
using ApplicationCore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Infrastructure.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   
   public AATestsController()
   {
     

   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      return Ok();
   }
}
