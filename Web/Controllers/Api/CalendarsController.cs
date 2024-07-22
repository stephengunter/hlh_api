
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
      private readonly ICalendarsService _calendarsService;
      private readonly IMapper _mapper;

      public CalendarsController(ICalendarsService calendarsService,
         IMapper mapper)
      {

         _calendarsService = calendarsService;
         _mapper = mapper;
      }

      [HttpGet]
      public async Task<ActionResult<IEnumerable<CalendarViewModel>>> Index()
      {
         var calendars = await _calendarsService.FetchAsync();
         calendars = calendars.GetOrdered();
         return calendars.MapViewModelList(_mapper);
      }
   }


}