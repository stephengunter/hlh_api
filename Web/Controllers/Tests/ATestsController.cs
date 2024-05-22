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
using ApplicationCore.Services.Auth;
using ApplicationCore.Views.Jud;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
   private readonly DefaultContext _defaultContext;
   private readonly IUsersService _usersService;

   public ATestsController(IUsersService usersService, DefaultContext defaultContext)
   {
      _defaultContext = defaultContext;
      _usersService = usersService;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var authToken = _defaultContext.AuthTokens.ToList();
      var entry = authToken[1];

      var user = await _usersService.FindByUsernameAsync(entry.UserName);
      if (user == null)
      {
         user = await _usersService.CreateAsync(new User
         {
            UserName = entry.UserName,
            Name = entry.UserName,
            SecurityStamp = Guid.NewGuid().ToString(),
            Active = true
         });
      }

      var adUsers = JsonConvert.DeserializeObject<List<AdUserViewModel>>(entry.AdListJson);
      if (adUsers!.HasItems())
      {
         var roles = adUsers.Select(item => item.ResolveRole()).Distinct();
         if (roles.HasItems())
         {
            foreach (string role in roles.Select(x => x.ToString()))
            {
               var hasRole = await _usersService.HasRoleAsync(user, role);
               if (!hasRole) await _usersService.AddToRoleAsync(user, role);
            }
         }
      } 
      

      return Ok();
   }


   [HttpGet("ex")]
   public ActionResult Ex()
   {
      throw new Exception("Test Throw Exception");
   }
}
