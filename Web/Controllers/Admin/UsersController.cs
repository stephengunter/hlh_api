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

namespace Web.Controllers.Admin;

public class UsersController : BaseAdminController
{
   private readonly IUsersService _usersService;
   private readonly IMapper _mapper;
   private readonly JudSettings _judSettings;
   private readonly AdminSettings _adminSettings;

   public UsersController(IUsersService usersService, IOptions<JudSettings> judSettings,
      IOptions<AdminSettings> adminSettings, IMapper mapper)
   {
      _usersService = usersService;
      _judSettings = judSettings.Value;
      _mapper = mapper;
      _adminSettings = adminSettings.Value;
   }
   [HttpGet("")]
   public async Task<ActionResult<UsersAdminModel>> Index(bool active, string? role, string? keyword, int page = 1, int pageSize = 10)
   {
      var request = new UsersAdminRequest(active, role, keyword, page, pageSize);
      var model = new UsersAdminModel(request);

      var roles = _usersService.FetchRoles();

      model.Roles = roles.MapViewModelList(_mapper);

      var selectedRole = GetRole(request, roles);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var users = await _usersService.FetchByRoleAsync(selectedRole);

      users = users.Where(u => u.Active == request.Active);

      var keywords = request.Keyword.GetKeywords();
      if (keywords.HasItems()) users = users.FilterByKeyword(keywords);

      
      model.PagedList = users.GetPagedList(_mapper, page, pageSize);

      return model;
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

   [HttpGet("roles")]
   public ActionResult<IEnumerable<RoleViewModel>> GetRoles()
   {
      var roles = _usersService.FetchRoles();
      return roles.MapViewModelList(_mapper);
   }
   [HttpPost("import")]
   public async Task<IActionResult> Import([FromForm] AdminFileRequest request)
   {
      ValidateRequest(request, _adminSettings);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      if (request.Files.Count < 1)
      {
         ModelState.AddModelError("files", "必須上傳檔案");
         return BadRequest(ModelState);
      }

      var file = request.Files.FirstOrDefault();
      if (Path.GetExtension(file!.FileName).ToLower() != ".csv")
      {
         ModelState.AddModelError("files", "檔案格式錯誤");
         return BadRequest(ModelState);
      }
      var users = new List<User>();
      var exLines = new List<string>();
      using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
      {
         // Skip the header line
         var header = reader.ReadLine();

         while (!reader.EndOfStream)
         {
            var line = reader.ReadLine();
            var parts = line!.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 3)
            {
               string name = parts[0].Trim();
               string type = parts[1].Trim();
               string title = parts[2].Trim();
               if (type == "使用者")
               {
                  string pattern = @"^(.*)\((.*)\)$";
                  var match = Regex.Match(title, pattern);

                  if (match.Success)
                  {
                     string part1 = match.Groups[1].Value; // Text outside the parentheses
                     string part2 = match.Groups[2].Value; // Text inside the parentheses

                     Console.WriteLine($"Part 1: {part1}");
                     Console.WriteLine($"Part 2: {part2}");

                     //var result = new[] { part1, part2 };  // Create an array of both parts
                  }
                  else
                  {
                     Console.WriteLine(title);
                  }

                  users.Add(new User
                  {
                     UserName = name,
                     Name = title,
                     Email = $"{name}@{_judSettings.Domain}",
                     LastUpdated = DateTime.Now
                  });
               }
               else
               {
                  exLines.Add(line);
               }

            }
            else
            {
               exLines.Add(line);
            }
         }
      }
      //foreach (var user in users) 
      //{
      //   var existingUser = await _usersService.FindByUsernameAsync(user.UserName!);
      //   if (existingUser is null)
      //   {
      //      await _usersService.CreateAsync(user);
      //   }
      //   else
      //   {
      //      existingUser.Email = user.Email;
      //      existingUser.Name = user.Name;
      //      await _usersService.UpdateAsync(existingUser);
      //   }
      //}

      return Ok(exLines);
   }

   Role? GetRole(UsersAdminRequest request, IEnumerable<Role> roles)
   {
      if (String.IsNullOrEmpty(request.Role)) return null;
      var role = roles.FirstOrDefault(role => request.Role.EqualTo(role.Name!));
      if (role is null)
      {
         ModelState.AddModelError("role", $"Role '{ request.Role }' not found.");
         return null;
      }
      return role;
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