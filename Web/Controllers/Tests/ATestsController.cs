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

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
  
   private readonly AppSettings _appSettings;
   private readonly IUsersService _usersService;
   private readonly IMapper _mapper;


   public ATestsController(IUsersService usersService, IOptions<AppSettings> appSettings, IMapper mapper)
   {
      _usersService = usersService;
      _mapper = mapper;
      _appSettings = appSettings.Value;
   }

   [HttpGet]
   public async Task<ActionResult> Index()
   {

      var input = "485ccc¤¤";
      var valid = input.IsAlphaNumeric();
     

      return Ok(valid);

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
