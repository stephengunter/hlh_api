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
using ApplicationCore.Consts;
using Infrastructure.Entities;
using System;
using ApplicationCore.Authorization;
using ApplicationCore.Exceptions;
using QuestPDF.Helpers;
using Azure.Core;
using ApplicationCore.Services.Files;
using Web.Helpers;
using Newtonsoft.Json;
using Ardalis.Specification;

namespace Web.Controllers.Api;
public class TagsController : BaseApiController
{
   private readonly ITagsService _tagsService;
   private readonly IMapper _mapper;

   public TagsController(ITagsService tagsService, IMapper mapper)
   {
      _tagsService = tagsService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<IEnumerable<Tag>>> Fetch(string? keyword = "")
   {
      var tags = await _tagsService.FetchAsync(keyword);
      return tags.ToList();
   }

}