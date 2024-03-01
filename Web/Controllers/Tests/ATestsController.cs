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

   public ATestsController(IOptions<AppSettings> appSettings, DefaultContext context)
   {
      _context = context;
      _appSettings = appSettings.Value;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var departments = _context.Departments.ToList();
      foreach (var department in departments)
      {
         if (department.Type == DepartmentTypes.GU)
         {
            await AddLocationIfNotExist(_context, new Location { Title = $"{department.Title}ªk©x«Ç", Key = department.Key, Order = department.Order, ParentId = 20 });
         }
        
      }
      _context.SaveChanges();
      return Ok();
   }
   async Task AddLocationIfNotExist(DefaultContext context, Location location)
   {
      var exist = await context.Locations.FirstOrDefaultAsync(x => x.Title == location.Title);
      if (exist == null) context.Locations.Add(location);
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
