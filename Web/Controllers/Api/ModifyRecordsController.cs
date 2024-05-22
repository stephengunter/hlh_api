
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
         var records = await _baseService.FetchModifyRecordsAsync(type, id, action);
         var modelList = new List<ModifyRecordViewModel>();
         if (records.HasItems())
         {
            var userIds = records.Select(x => x.UserId).Distinct();
            var users = await _usersService.FetchByIdsAsync(userIds);
            modelList = records.GetOrdered().MapViewModelList(_mapper);
            foreach (var user in users) 
            {
               var models = modelList.Where(x => x.UserId == user.Id);
               foreach (var model in models) model!.UserName = user.UserName!;

            }

         }
         return modelList;
      }

   }


}