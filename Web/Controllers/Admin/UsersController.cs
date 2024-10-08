using ApplicationCore.Services;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Infrastructure.Helpers;
using Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.Views;
using Microsoft.IdentityModel.Tokens;
using ApplicationCore.Authorization;
using System.Text.RegularExpressions;
using System.IO;
using Infrastructure.Paging;
using ApplicationCore.Services.Auth;
using ApplicationCore;
using ApplicationCore.Views.AD;

namespace Web.Controllers.Admin;

public class UsersController : BaseAdminController
{
   private readonly IUsersService _usersService;
   private readonly IProfilesService _profilesService;
   private readonly ILdapService _ldapService;
   private readonly IDepartmentsService _departmentsService;
   private readonly IMapper _mapper;
   private readonly AdminSettings _adminSettings;

   public UsersController(IUsersService usersService, IProfilesService profilesService, IOptions<LdapSettings> ldapSettings,
      IDepartmentsService departmentsService, IOptions<AdminSettings> adminSettings, IMapper mapper)
   {
      _usersService = usersService;
      _profilesService = profilesService;
      _departmentsService = departmentsService;
      _mapper = mapper;
      _adminSettings = adminSettings.Value;

      _ldapService = Factories.CreateLdapService(ldapSettings.Value);
   }
   [HttpGet("init")]
   public async Task<ActionResult<UsersAdminModel>> Init()
   {
      bool active = true;
      int department = 0;
      string role = "";
      string keyword = "";
      int page = 1;
      int pageSize = 10;

      var request = new UsersAdminRequest(active, department, role, keyword, page, pageSize);

      var roles = await _usersService.FetchRolesAsync();
      var departments = await _departmentsService.FetchAllAsync();

      return new UsersAdminModel(request, roles.MapViewModelList(_mapper), departments.MapViewModelList(_mapper));
   }

   [HttpGet]
   public async Task<ActionResult<PagedList<User, UserViewModel>>> Index(bool active, int? department, string? role, string? keyword, int page = 1, int pageSize = 10)
   {
      bool includeRoles = true;
      var roleNames = new List<string>();
      if (!string.IsNullOrEmpty(role)) roleNames = role.Split(',').ToList();

      Department? selectedDepartment = null;
      if (department.HasValue && department.Value > 0)
      {
         selectedDepartment = await _departmentsService.GetByIdAsync(department.Value);
         if (selectedDepartment == null) ModelState.AddModelError("department", $"department not found. id: {department.Value}");
      }
      if (!ModelState.IsValid) return BadRequest(ModelState);

      IEnumerable<User> users;
      if (roleNames.Count == 0)
      {
         users = await _usersService.FetchAllAsync(includeRoles);
      }
      else
      {
         var selectedRoles = new List<Role>();
         foreach (var roleName in roleNames)
         {
            var selectedRole = await _usersService.FindRoleAsync(roleName);
            if (selectedRole == null) ModelState.AddModelError("role", $"Role '{roleName}' not found.");
            else selectedRoles.Add(selectedRole);
         }
         if (!ModelState.IsValid) return BadRequest(ModelState);

         users = await _usersService.FetchByRolesAsync(selectedRoles, includeRoles);
      }

      if (selectedDepartment != null)
      {
         var profiles = await _profilesService.FetchAsync(selectedDepartment);
         var userIds = profiles.Select(x => x.UserId).ToList();
         if(userIds.HasItems()) users = users.Where(u => userIds.Contains(u.Id));
      }           

      users = users.Where(u => u.Active == active);

      var keywords = keyword.GetKeywords();
      if (keywords.HasItems()) users = users.FilterByKeyword(keywords);

      return users.GetPagedList(_mapper, page, pageSize);
   }

   [HttpGet("create")]
   public ActionResult<UserViewModel> Create() => new UserViewModel();

   [HttpPost]
   public async Task<ActionResult<UserViewModel>> Store([FromBody] UserViewModel model)
   {
      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var user = new User()
      {
         UserName = model.UserName,
         Email = model.Email,
         Name = model.Name,
         CreatedAt = DateTime.Now,
         LastUpdated = DateTime.Now,
         Active = model.Active
      };

      user = await _usersService.CreateAsync(user);

      return Ok(user.MapViewModel(_mapper));
   }

   [HttpGet("{id}")]
   public async Task<ActionResult<UserViewModel>> Details(string id)
   {
      var user = await _usersService.GetByIdAsync(id);
      if (user == null) return NotFound();

      return user.MapViewModel(_mapper);
   }

   [HttpGet("edit/{id}")]
   public async Task<ActionResult<UserViewModel>> Edit(string id)
   {
      var user = await _usersService.FindByIdAsync(id);
      if (user == null) return NotFound();

      return user.MapViewModel(_mapper);
   }
   [HttpPut("{id}")]
   public async Task<ActionResult> Update(string id, [FromBody] UserViewModel model)
   {
      var user = await _usersService.FindByIdAsync(id);
      if (user == null) return NotFound();

      await ValidateRequestAsync(model);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      user = model.MapEntity(_mapper, User.Id(), user);

      await _usersService.UpdateAsync(user);

      return NoContent();
   }
   [HttpPost("sync")]
   public async Task<IActionResult> Sync([FromBody] AdminRequest request)
   {
      ValidateRequest(request, _adminSettings);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var adUsers = _ldapService.FetchAll();
      adUsers = adUsers.Where(x => !String.IsNullOrEmpty(x.Department));
      foreach (var aduser in adUsers) 
      { 
         var department = await _departmentsService.FindByTitleAsync(aduser.Department);
         if (department == null) continue;

         var user = await _usersService.FindByUsernameAsync(aduser.Username);
         if (user == null)
         {
            user = await _usersService.CreateAsync(new User
            {
               UserName = aduser.Username,
               Name = aduser.Username,
               SecurityStamp = Guid.NewGuid().ToString(),
               Active = true,
               CreatedAt = DateTime.Now,
               CreatedBy = User.Id()
            });
         }

         var profiles = await _profilesService.FindAsync(user);
         if (profiles == null)
         {
            profiles = await _profilesService.CreateAsync(new Profiles
            {
               UserId = user.Id,
               Name = aduser.Title,
               DepartmentId = department.Id,
               CreatedAt = DateTime.Now,
               CreatedBy = User.Id()
            });
         }
         else
         {
            profiles.Name = aduser.Title;
            profiles.DepartmentId = department.Id;
            profiles.LastUpdated = DateTime.Now;
            profiles.UpdatedBy = User.Id();

            await _profilesService.UpdateAsync(profiles);
         }

      }

      return Ok();
   }


   async Task ValidateRequestAsync(UserViewModel model)
   {
      await CheckUserNameAsync(model);
      await CheckEmailAsync(model);
   }

   async Task CheckUserNameAsync(UserViewModel model)
   {
      if(String.IsNullOrEmpty(model.UserName)) ModelState.AddModelError("userName", "必須填寫userName");
      if(!model.UserName.IsValidUserName()) ModelState.AddModelError("userName", "userName的格式不正確");

      var existingUser = await _usersService.FindByUsernameAsync(model.UserName);
      if(existingUser != null && existingUser.Id != model.Id) ModelState.AddModelError("userName", "userName重複了");
   }
   async Task CheckEmailAsync(UserViewModel model)
   {
      if (String.IsNullOrEmpty(model.Email)) ModelState.AddModelError("email", "必須填寫email");
      if (!model.Email.IsValidEmail()) ModelState.AddModelError("email", "email的格式不正確");

      var existingUser = await _usersService.FindByEmailAsync(model.Email);
      if (existingUser != null && existingUser.Id != model.Id) ModelState.AddModelError("email", "email重複了");
   }

}