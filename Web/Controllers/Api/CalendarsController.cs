
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Web.Models;

namespace Web.Controllers.Api
{
   public class CalendarsController : BaseApiController
   {
      private readonly ICategorysService _categorysService;
      private readonly IMapper _mapper;

      public CalendarsController(ICategorysService categorysService, 
         IMapper mapper)
      {

         _categorysService = categorysService;
         _mapper = mapper;
      }
      

   }


}