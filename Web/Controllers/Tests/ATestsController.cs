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
using ApplicationCore.Settings.Files;
using FluentFTP;
using System.Runtime;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
  
   private readonly JudgebookFileSettings _settings;
   private readonly UserManager<User> _userManager;

   public ATestsController(IOptions<JudgebookFileSettings> settings, UserManager<User> userManager)
   {
      _userManager = userManager;
      _settings = settings.Value;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      
      var client = new AsyncFtpClient(_settings.Host, _settings.UserName, _settings.Password);

      await client.AutoConnect();

      foreach (FtpListItem item in await client.GetListing(_settings.Directory))
      {

         // if this is a file
         if (item.Type == FtpObjectType.File)
         {

            // get the file size
            long size = await client.GetFileSize(item.FullName);

            // calculate a hash for the file on the server side (default algorithm)
            FtpHash hash = await client.GetChecksum(item.FullName);
         }

         // get modified date/time of the file or folder
         DateTime time = await client.GetModifiedTime(item.FullName);
      }
      await client.Disconnect();

      return Ok();
   }
   


   //[HttpGet("version")]
   //public ActionResult Version()
   //{
   //   return Ok(_appSettings.ApiVersion);
   //}


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
