using ApplicationCore.Models;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using ApplicationCore.Services;
using ApplicationCore.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Models;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ApplicationCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using ApplicationCore.Views;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using ApplicationCore.DataAccess;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting.Server;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
  
   private readonly AppSettings _appSettings;
   private readonly DefaultContext _context;
   private readonly IWebHostEnvironment _environment;

   public ATestsController(IWebHostEnvironment environment, IOptions<AppSettings> appSettings, DefaultContext context)
   {
      _environment = environment;
      _context = context;
      _appSettings = appSettings.Value;
      _environment = environment;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var path = GetTempPath(_environment, DateTime.Today.ToDateNumber().ToString());
      var filePath = Path.Combine(GetTempPath(_environment, DateTime.Today.ToDateNumber().ToString()), "departments.json");

      // Check if the file exists
      if (!System.IO.File.Exists(filePath))
      {
         return NotFound(); // Return 404 if the file does not exist
      }
      // Example stream (replace with your own)
      Stream stream = new FileStream(filePath, FileMode.Open);

      // Return the stream as a file
      return File(stream, "application/octet-stream", "departments.json");
   }

   [HttpGet("version")]
   public ActionResult Version()
   {
      return Ok(_appSettings.ApiVersion);
   }


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
