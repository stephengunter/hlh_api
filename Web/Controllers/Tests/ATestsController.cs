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
using ApplicationCore.Services.Files;
using Web.Models.Files;
using ApplicationCore.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ApplicationCore.Models.Files;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{

   private readonly JudgebookFileSettings _judgebookSettings;
   private readonly IFileStoragesService _fileStoragesService;

   public ATestsController(IOptions<JudgebookFileSettings> judgebookSettings, IFileStoragesService fileStoragesService)
   {     
      _judgebookSettings = judgebookSettings.Value;
      _fileStoragesService = fileStoragesService;
      _fileStoragesService = new FtpStoragesService(_judgebookSettings.Host, _judgebookSettings.UserName,
            _judgebookSettings.Password, _judgebookSettings.Directory);
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      return Ok();
   }


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
