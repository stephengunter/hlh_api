using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using Web.Models;
using ApplicationCore.Exceptions;
using ApplicationCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Infrastructure.Helpers;
using ApplicationCore.Models.Auth;
using ApplicationCore.Services.Auth;
using System.Data;
using ApplicationCore.Views.Jud;
using Newtonsoft.Json;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;


[EnableCors("Open")]
public class TelsController : BaseController
{
   private readonly DefaultContext _defaultContext;

   public TelsController(DefaultContext defaultContext)
	{
      _defaultContext = defaultContext;
   }

   [HttpGet]
	public async Task<ActionResult> Index()
	{
      var list = await _defaultContext.TelNames.ToListAsync();

      return Ok(list);
   }

}

