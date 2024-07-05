using ApplicationCore.Models;
using ApplicationCore.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Infrastructure.Helpers;
namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly IReferenceService _referenceService;
   private readonly IAttachmentService _attachmentService;
   private readonly IMapper _mapper;

   public AATestsController(IReferenceService referenceService, IAttachmentService attachmentService, IMapper mapper)
   {
      _referenceService = referenceService;
      _attachmentService = attachmentService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      var entity = new Tasks { Id = 3 };
      var refs = await _referenceService.FetchAsync();
      return Ok(refs.MapViewModelList(_mapper));
   }
}
