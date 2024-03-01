using ApplicationCore.Services;
using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.DtoMapper;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace Web.Controllers.Admin;

public class JobUserProfilesController : BaseAdminController
{
   private readonly IJobUserProfilessService _service;
   private readonly IProfilesService _profilesService;
   private readonly IJobsService _jobsService;
   private readonly IMapper _mapper;

  
   public JobUserProfilesController(IJobUserProfilessService service, IProfilesService profilesService,
      IJobsService jobsService, IMapper mapper)
   {
      _service = service;
      _profilesService = profilesService;
      _jobsService = jobsService;
      _mapper = mapper;
   }
   [HttpGet("user/{userId}")]
   public async Task<ActionResult<IEnumerable<JobUserProfilesViewModel>>> FetchByUser(string userId)
   {
      if (String.IsNullOrEmpty(userId))
      {
         ModelState.AddModelError("userId", "必須有userId");
         return BadRequest(ModelState);
      }
      var profiles = await _profilesService.FindAsync(new ApplicationCore.Models.User { Id = userId });
      if (profiles is null) return NotFound();


      var records = await _service.FetchAsync(new ApplicationCore.Models.User { Id = userId });
      return records.MapViewModelList(_mapper);
   }

   [HttpGet("job/{id}")]
   public async Task<ActionResult<IEnumerable<JobUserProfilesViewModel>>> FetchByJob(int id)
   {
      var job = await _jobsService.GetByIdAsync(id);
      if (job is null) return NotFound();

      var records = await _service.FetchAsync(job);
      return records.MapViewModelList(_mapper);
   }

   [HttpGet("create/{jobId}")]
   public ActionResult<JobUserProfilesViewModel> Create(int jobId)
   {
      return new JobUserProfilesViewModel() { JobId = jobId };
   }


   [HttpPost]
   public async Task<ActionResult<JobUserProfilesViewModel>> Store([FromBody] JobUserProfilesViewModel model)
   {
      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = model.MapEntity(_mapper, User.Id());
      entity.StartDate = model.StartDateText.ToStartDate();
      entity.EndDate = model.EndDateText.ToEndDate();

      entity = await _service.CreateAsync(entity);

      return Ok(entity.MapViewModel(_mapper));
   }

   [HttpGet("{id}")]
   public async Task<ActionResult<JobUserProfilesViewModel>> Details(int id)
   {
      var entity = await _service.DetailsAsync(id);
      if (entity == null) return NotFound();

      return entity.MapViewModel(_mapper);
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult<JobUserProfilesViewModel>> Edit(int id)
   {
      var entity = await _service.GetByIdAsync(id);
      if (entity == null) return NotFound();

      return entity.MapViewModel(_mapper);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] JobUserProfilesViewModel model)
   {
      var entity = await _service.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      //entity = model.MapEntity(_mapper, User.Id(), entity);
      //entity.StartDate = model.StartDateText.ToStartDate();
      //entity.EndDate = model.EndDateText.ToStartDate();

      entity.StartDate = new DateTime(2023, 10, 1);
      entity.EndDate = new DateTime(2023, 10, 1);
      entity.PS += "pp";
      await _service.UpdateAsync(entity);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _service.GetByIdAsync(id);
      if (entity == null) return NotFound();

      entity.Removed = true;
      await _service.UpdateAsync(entity);

      return NoContent();
   }

   async Task ValidateRequestAsync(JobUserProfilesViewModel model)
   {
      await CheckUserAsync(model);
      await CheckJobAsync(model);
   }

   async Task CheckUserAsync(JobUserProfilesViewModel model)
   {
      if (String.IsNullOrEmpty(model.UserId))
      {
         ModelState.AddModelError("userId", "必須有userId");
         return;
      }
      var profiles = await _profilesService.FindAsync(new ApplicationCore.Models.User { Id = model.UserId });
      if (profiles is null) ModelState.AddModelError("profiles", "查無此個人資料");
   }
   async Task CheckJobAsync(JobUserProfilesViewModel model)
   {
      var job = await _jobsService.GetByIdAsync(model.JobId);
      if (job is null) ModelState.AddModelError("job", "查無此職缺");
   }

}