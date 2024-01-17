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

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly IUsersService _usersService;
   private readonly UserManager<User> _userManager;
   private readonly AppSettings _appSettings;

   public ATestsController(IUsersService usersService, UserManager<User> userManager, IOptions<AppSettings> appSettings)
   {
      _usersService = usersService;
      _userManager = userManager;
      _appSettings = appSettings.Value;
   }

   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var x = User.Claims.IsNullOrEmpty();
      var Roles = User.Roles();
      var IsDev = User.IsDev();

      return Ok(Roles.JoinToString() + "," + IsDev);

      
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
