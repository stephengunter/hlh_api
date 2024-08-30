using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Microsoft.AspNetCore.Cors;
using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Models;
using System.Collections.Generic;
using ApplicationCore.Views;

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
   public async Task<ActionResult<IEnumerable<UnitPerson>>> Init(int flag)
   {
      var unitPersons = await _defaultContext.UnitPersons.Where(x => x.Flag == flag).OrderBy(x => x.Person).ToListAsync();
      return unitPersons;
   }

   [HttpGet]
   public async Task<ActionResult<IEnumerable<DocModel>>> Index(int flag, string person)
	{
      if (flag < -1 || flag > 2) return BadRequest("Wrong Flag.");
      if (string.IsNullOrEmpty(person)) return BadRequest("person has no value.");

      if (person == "null")
      {
         var records = _defaultContext.DocModels.Where(x => x.Flag == flag && string.IsNullOrEmpty(x.Person)).ToList();
         return records.OrderByDescending(x => x.DateNumber).OrderByDescending(x => x.Year).ToList();
      }
      else
      {
         var records = _defaultContext.DocModels.Where(x => x.Flag == flag && x.Person == person.Trim()).ToList();

         return records.OrderByDescending(x => x.DateNumber).OrderByDescending(x => x.Year).ToList();
      }

      

   }
   [HttpPost]
   public async Task<ActionResult<IEnumerable<UnitPerson>>> Store([FromBody] DocKeepRequest model)
   {
      var record = await _defaultContext.DocModels.Where(x => x.Flag == model.Flag && x.Person == model.Person.Trim()).ToListAsync();
      foreach (var item in record) 
      { 
         var keep = model.DocModels.FirstOrDefault(x => x.Id == item.Id);
         if (keep != null)
         {
            item.Keep = keep.Keep;
         }
         else item.Keep = 0;

         _defaultContext.DocModels.Update(item);
      }
      _defaultContext.SaveChanges();

      var uperson = _defaultContext.UnitPersons.FirstOrDefault(x => x.Flag == model.Flag && x.Person == model.Person.Trim());
      if (uperson != null) 
      {
         uperson.Confirmed = true;
         uperson.LastUpdated = DateTime.Now;
         uperson.Saves += 1;
         uperson.Ip = RemoteIpAddress;
         _defaultContext.UnitPersons.Update(uperson);
         _defaultContext.SaveChanges();
      }
      var unitPersons = await _defaultContext.UnitPersons.Where(x => x.Flag == model.Flag).OrderBy(x => x.Person).ToListAsync();
      return unitPersons!;
   }
}

