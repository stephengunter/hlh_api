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

public class ProfilesController : BaseAdminController
{
   private readonly IUsersService _usersService;
   private readonly IProfilesService _profilesService;
   private readonly IMapper _mapper;

  
   public ProfilesController(IUsersService usersService, IProfilesService profilesService, IMapper mapper)
   {
      _usersService = usersService;
      _profilesService = profilesService;
      _mapper = mapper;
   }

   [HttpGet]
   public async Task<ActionResult<IEnumerable<ProfilesViewModel>>> Fetch()
   {
      var profiles = await _profilesService.FetchAsync();

      return profiles.MapViewModelList(_mapper);
   }

   [HttpGet("create/{id}")]
   public async Task<ActionResult<ProfilesViewModel>> Create(string id)
   {
      var user = await _usersService.FindByIdAsync(id);
      if (user == null) return NotFound();

      var profiles = await _profilesService.FindAsync(user);
      if (profiles != null) return NotFound();

      return new ProfilesViewModel() { UserId = id };
   }

   [HttpPost]
   public async Task<ActionResult<ProfilesViewModel>> Store([FromBody] ProfilesViewModel model)
   {
      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var profiles = model.MapEntity(_mapper, User.Id());
      profiles.CreatedAt = DateTime.Now;
      profiles.LastUpdated = DateTime.Now;
      profiles.UpdatedBy = User.Id();

      profiles = await _profilesService.CreateAsync(profiles);

      return Ok(profiles.MapViewModel(_mapper));
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult> Edit(string id)
   {
      var profiles = await _profilesService.FindAsync(new User { Id = id });
      if (profiles == null) return NotFound();

      var model = profiles.MapViewModel(_mapper);

      return Ok(model);
   }

   [HttpPut("{id}")]
   public async Task<ActionResult> Update(string id, [FromBody] ProfilesViewModel model)
   {
      var profiles = await _profilesService.FindAsync(new User { Id = id });
      if (profiles == null) return NotFound();

      ValidateRequest(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      profiles = model.MapEntity(_mapper, User.Id(), profiles);
      profiles.LastUpdated = DateTime.Now;
      profiles.UpdatedBy = User.Id();

      await _profilesService.UpdateAsync(profiles);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Delete(string id)
   {
      var profiles = await _profilesService.FindAsync(new User { Id = id });
      if (profiles == null) return NotFound();
      
      await _profilesService.DeleteAsync(profiles);

      return NoContent();
   }

   void ValidateRequest(ProfilesViewModel model)
   {
      if (String.IsNullOrEmpty(model.Name)) ModelState.AddModelError("name", "必須填寫姓名");      

   }


}