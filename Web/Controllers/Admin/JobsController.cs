using ApplicationCore.Services;
using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.DtoMapper;
using ApplicationCore.Authorization;
using Infrastructure.Helpers;

namespace Web.Controllers.Admin;

public class JobsController : BaseAdminController
{
   private readonly IJobsService _jobsService;
   private readonly IMapper _mapper;

  
   public JobsController(IJobsService jobsService, IDepartmentsService departmentsService, IMapper mapper)
   {
      _jobsService = jobsService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult<IEnumerable<JobViewModel>>> Index(string depIds)
   {
      if (String.IsNullOrEmpty(depIds))
      {
         ModelState.AddModelError("depIds", "必須有depIds");
         return BadRequest(ModelState);
      }
      var departments = depIds.SplitToIntList().Select(id => new Department { Id = id });
      var jobs = await _jobsService.FetchAsync(departments);
      return jobs.MapViewModelList(_mapper);
   }

   [HttpGet("create")]
   public ActionResult<JobViewModel> Create() => new JobViewModel() { Active = true };


   [HttpPost]
   public async Task<ActionResult<JobViewModel>> Store([FromBody] JobViewModel model)
   {
      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var job = model.MapEntity(_mapper, User.Id());
      job.Order = model.Active ? 0 : -1;

      job = await _jobsService.CreateAsync(job);

      return Ok(job.MapViewModel(_mapper));
   }

   [HttpGet("{id}")]
   public async Task<ActionResult<JobViewModel>> Details(int id)
   {
      var job = await _jobsService.DetailsAsync(id);
      if (job == null) return NotFound();

      return job.MapViewModel(_mapper);
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult<JobViewModel>> Edit(int id)
   {
      var job = await _jobsService.GetByIdAsync(id);
      if (job == null) return NotFound();

      return job.MapViewModel(_mapper);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] JobViewModel model)
   {
      var job = await _jobsService.GetByIdAsync(id);
      if (job == null) return NotFound();

      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      job = model.MapEntity(_mapper, User.Id(), job);

      await _jobsService.UpdateAsync(job);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var job = await _jobsService.GetByIdAsync(id);
      if (job == null) return NotFound();

      job.Removed = true;
      job.Order = -1;
      await _jobsService.UpdateAsync(job);

      return NoContent();
   }

   void ValidateRequest(JobViewModel model)
   {
      //if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "������g���D");      

   }


}