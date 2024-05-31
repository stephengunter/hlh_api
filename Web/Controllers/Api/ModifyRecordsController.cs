
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Views;
using Infrastructure.Helpers;

namespace Web.Controllers.Api
{
   public class ModifyRecordsController : BaseApiController
   {
      private readonly IBaseService _baseService;
      private readonly IUsersService _usersService;
      private readonly IMapper _mapper;

      public ModifyRecordsController(IBaseService baseService, IUsersService usersService, IMapper mapper)
      {
         _baseService = baseService;
         _usersService = usersService;
         _mapper = mapper;
      }

      [HttpGet]
      public async Task<ActionResult<List<ModifyRecordViewModel>>> Fetch(string type, string id, string action = "")
      {
         var records = await _baseService.FetchModifyRecordsAsync(type, id, action.SplitToList());
         var modelList = await records.MapViewModelListAsync(_usersService, _mapper);
         return modelList;
      }

   }


}