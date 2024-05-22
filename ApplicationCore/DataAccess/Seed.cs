using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ApplicationCore.Models;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ApplicationCore.Models.Files;

namespace ApplicationCore.DataAccess;

public static class SeedData
{
	
	static string DevRoleName = AppRoles.Dev.ToString();
	static string BossRoleName = AppRoles.Boss.ToString();
   static string ITRoleName = AppRoles.IT.ToString();
   static string RecorderRoleName = AppRoles.Recorder.ToString();
   static string ClerkRoleName = AppRoles.Clerk.ToString();
   static string FilesRoleName = AppRoles.Files.ToString();

   public static async Task EnsureSeedData(IServiceProvider serviceProvider, ConfigurationManager Configuration)
	{
		string adminEmail = Configuration[$"{SettingsKeys.Admin}:Email"] ?? "";
		string adminPhone = Configuration[$"{SettingsKeys.Admin}:Phone"] ?? "";
		string adminName = Configuration[$"{SettingsKeys.Admin}:Name"] ?? "";

		if(String.IsNullOrEmpty(adminEmail) || String.IsNullOrEmpty(adminPhone))
		{
			throw new Exception("Failed to SeedData. Empty Admin Email/Phone.");
		}
		if(String.IsNullOrEmpty(adminName))
		{
			throw new Exception("Failed to SeedData. Empty Admin Name.");
		}

		Console.WriteLine("Seeding database...");

		var context = serviceProvider.GetRequiredService<DefaultContext>();
	   context.Database.EnsureCreated();
		using (var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>())
		{
			await SeedRoles(roleManager);
		}

		if(!String.IsNullOrEmpty(adminEmail)) {
			using (var userManager = serviceProvider.GetRequiredService<UserManager<User>>())
			{
				var user = new User
				{
					Email = adminEmail,			
					UserName = adminEmail,
					Name = adminName,
					PhoneNumber = adminPhone,
					EmailConfirmed = true,
					SecurityStamp = Guid.NewGuid().ToString(),
					Active = true
				};
				await CreateUserIfNotExist(userManager, user, new List<string>() { DevRoleName });
			}
		}
      await SeedCategories(context);
      await SeedDepartments(context);
      await SeedJobTitles(context);
		await SeedLocations(context);
      await SeedJudgebookTypes(context);
      Console.WriteLine("Done seeding database.");
	}

	static async Task SeedRoles(RoleManager<Role> roleManager)
	{
		var roles = new List<Role> 
		{ 
			new Role { Name = DevRoleName, Title = "開發者" },
         new Role { Name = BossRoleName, Title = "老闆" },
         new Role { Name = ITRoleName, Title = "資訊人員" },
         new Role { Name = RecorderRoleName, Title = "錄事" },
         new Role { Name = ClerkRoleName, Title = "書記官" },
         new Role { Name = FilesRoleName, Title = "檔案管理員" }
      };
		foreach (var item in roles) await AddRoleIfNotExist(roleManager, item);
	}
	static async Task AddRoleIfNotExist(RoleManager<Role> roleManager, Role role)
	{
		var existingRole = await roleManager.FindByNameAsync(role.Name!);
		if (existingRole == null) await roleManager.CreateAsync(role);
		else
		{
         existingRole.Title = role.Title;
			await roleManager.UpdateAsync(existingRole);
      } 

   }
	static async Task CreateUserIfNotExist(UserManager<User> userManager, User newUser, IList<string>? roles = null)
	{
		var user = await userManager.FindByEmailAsync(newUser.Email!);
		if (user == null)
		{
			var result = await userManager.CreateAsync(newUser);

			if (roles!.HasItems())
			{
				await userManager.AddToRolesAsync(newUser, roles!);
			}
		}
		else
		{
			user.PhoneNumber = newUser.PhoneNumber;
			user.Name = newUser.Name;
			await userManager.UpdateAsync(user);
			if (roles!.HasItems())
			{
				foreach (var role in roles!)
				{
					bool hasRole = await userManager.IsInRoleAsync(user, role);
					if (!hasRole) await userManager.AddToRoleAsync(user, role);
				}
			}

		}
	}

   static async Task SeedCategories(DefaultContext context)
   {
      var categories = new List<Category>
      {
         new Category() { Title = "", Key = PostType.Event.ToString(), PostType = PostType.Event },
         new Category() { Title = "", Key = PostType.Article.ToString(), PostType = PostType.Article },
      };
      foreach (var item in categories) await AddCategoryIfNotExist(context, item);
      context.SaveChanges();
   }
   static async Task AddCategoryIfNotExist(DefaultContext context, Category category)
   {
      if (context.Categories.Count() == 0)
      {
         context.Categories.Add(category);
         return;
      }
      var exist = await context.Categories.FirstOrDefaultAsync(x => x.Key == category.Key);
      if (exist == null) context.Categories.Add(category);
   }

   static async Task SeedDepartments(DefaultContext context)
   {
		var departments = new List<Department>
		{
			new Department() { Title = "司法院", Key = "JUD" },
         new Department() { Title = "花蓮高分院", Key = "HLH" },
      };
      foreach (var item in departments) await AddDepartmentIfNotExist(context, item);
		context.SaveChanges();
   }
   static async Task AddDepartmentIfNotExist(DefaultContext context, Department department)
   {
		if (context.Departments.Count() == 0) 
		{
         context.Departments.Add(department);
         return;
      }
		var exist = await context.Departments.FirstOrDefaultAsync(x => x.Key == department.Key);
		if (exist == null) context.Departments.Add(department);
   }
   static async Task SeedLocations(DefaultContext context)
   {
      var locations = new List<Location>
      {
         new Location() { Title = "行政大樓", Key = "AB" },
         new Location() { Title = "法庭大樓", Key = "CB" },
      };
      foreach (var item in locations) await AddLocationIfNotExist(context, item);
      context.SaveChanges();
   }
   static async Task AddLocationIfNotExist(DefaultContext context, Location location)
   {
      if (context.Locations.Count() == 0)
      {
         context.Locations.Add(location);
         return;
      }
      var exist = await context.Locations.FirstOrDefaultAsync(x => x.Title == location.Title);
      if (exist == null) context.Locations.Add(location);
   }
   static async Task SeedJobTitles(DefaultContext context)
   {
      var titles = new List<string>
      {
         "院長","庭長","審判長","法官","法官助理",
         "官長","科長","主任","管理師","操作員","科員",
         "書記官","庭務員","執達員","錄事","通譯",
         "工友","駕駛","派駐人員","資訊委外",
         "法警長","副法警長","法警"
      };

      await titles.ForEachWithIndexAsync(async (title, index) =>
      {
         await AddJobTitleIfNotExist(context, new JobTitle { Title = title, Order = index });
      });
      context.SaveChanges();
   }
   static async Task AddJobTitleIfNotExist(DefaultContext context, JobTitle jobTitle)
   {
      if (context.JobTitles.Count() == 0)
      {
         context.JobTitles.Add(jobTitle);
         return;
      }
      var exist = await context.JobTitles.FirstOrDefaultAsync(x => x.Title == jobTitle.Title);
		if (exist == null) context.JobTitles.Add(jobTitle);
		else exist.Order = jobTitle.Order;
   }
   static async Task SeedJudgebookTypes(DefaultContext context)
   {
      var types = new List<JudgebookType>
      {
         new JudgebookType() { Key = "JU", Title = "判決", Order = 0 },
         new JudgebookType() { Key = "RU", Title = "裁定", Order = 1 },
         new JudgebookType() { Key = "ST", Title = "和解筆錄",Order = 2 }
      };

		foreach (var item in types)
		{
			await AddJudgebookTypeIfNotExist(context, item);
      }
      context.SaveChanges();
   }
   static async Task AddJudgebookTypeIfNotExist(DefaultContext context, JudgebookType type)
	{
		if (context.JudgebookTypes.Count() == 0)
		{
			context.JudgebookTypes.Add(type);
			return;
		}
		var exist = await context.JudgebookTypes.FirstOrDefaultAsync(x => x.Title == type.Title);
		if (exist == null) context.JudgebookTypes.Add(type);
		else
		{
         exist.Key = type.Key;
         exist.Order = type.Order;
      } 
	}
}