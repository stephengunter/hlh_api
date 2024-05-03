
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Helpers;
using ApplicationCore.Services.Files;
using Infrastructure.Interfaces;

namespace Web.Controllers.Api
{
   public class ModifyRecordsController : BaseApiController
   {
      private readonly IBaseService _baseService;
      private readonly IMapper _mapper;

      public ModifyRecordsController(IBaseService baseService, IMapper mapper)
      {
         _baseService = baseService;
         _mapper = mapper;
      }

      [HttpGet("{type}/{id}")]
      public async Task<ActionResult<List<ModifyRecordViewModel>>> Fetch(string type, string id)
      {
         var records = await _baseService.FetchModifyRecordsAsync(type, id);
         return records.GetOrdered().MapViewModelList(_mapper);
      }

   }


}