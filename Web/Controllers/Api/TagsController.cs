
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Helpers;

namespace Web.Controllers.Api
{
   
   public class TagsController : BaseApiController
   {
      private readonly ITagsService _tagsService;
      private readonly IMapper _mapper;

      public TagsController(ITagsService tagsService, IMapper mapper)
      {
         _tagsService = tagsService;
         _mapper = mapper;
      }


      [HttpGet("")]
      public async Task<ActionResult<IEnumerable<TagViewModel>>> Index(int page = 1, int pageSize = 99)
      {
         if (page < 1) page = 1;

         var tags = await _tagsService.FetchAllAsync();

         tags = tags.Where(x => x.Active);

         tags = tags.GetOrdered().GetPaged(page, pageSize);

         return tags.MapViewModelList(_mapper);
      }

   }


}