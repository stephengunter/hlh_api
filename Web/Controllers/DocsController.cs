using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Microsoft.AspNetCore.Cors;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;


[EnableCors("Open")]
public class DocsController : BaseController
{
   private readonly DefaultContext _defaultContext;

   public DocsController(DefaultContext defaultContext)
	{
      _defaultContext = defaultContext;
   }

   [HttpGet("init")]
   public async Task<ActionResult<DocsIndexModel>> Init()
   {
      var list = await _defaultContext.TelNames.ToListAsync();
      var model = new DocsIndexModel();
      model.Departments = list;
      return model;
   }

   [HttpGet]
	public async Task<ActionResult> Index()
	{
      var list = await _defaultContext.TelNames.ToListAsync();

      return Ok(list);
   }

}

