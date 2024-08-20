using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ApplicationCore.Models;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

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
      await SeedCalendars(context);
      await SeedDepartments(context);
      await SeedJobTitles(context);
		await SeedLocations(context);
      await SeedCourts(context);
      await SeedTags(context);
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
      var rootCategories = new List<Category>
      {
         new Category() { Title = "", Key = PostTypes.Event, PostType = PostTypes.Event },
         new Category() { Title = "", Key = PostTypes.Article, PostType = PostTypes.Article }
      };
      foreach (var item in rootCategories) await AddCategoryIfNotExist(context, item);
      context.SaveChanges();

      await SeedEventCategories(context);
   }
   static async Task SeedEventCategories(DefaultContext context)
   {
      var root = context.Categories.FirstOrDefault(c => c.Key == PostTypes.Event && c.PostType == PostTypes.Event);
      var categories = new List<Category>
      {
         new Category() { Title = "教育訓練", Key = "ED", ParentId = root!.Id, PostType = PostTypes.Event },
         new Category() { Title = "遠距詢問", Key = "FD", ParentId = root.Id , PostType = PostTypes.Event },
         new Category() { Title = "雙向詢問", Key = "DW", ParentId = root.Id , PostType = PostTypes.Event },
         new Category() { Title = "U會議詢問", Key = "UM", ParentId = root.Id , PostType = PostTypes.Event }
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
      var exist = await context.Categories.FirstOrDefaultAsync(x => x.Key == category.Key && x.ParentId == category.ParentId);
      if (exist == null) context.Categories.Add(category);
      else 
      {
         exist.PostType = category.PostType;
      }
   }
   static async Task SeedCalendars(DefaultContext context)
   {
      var calendars = new List<Calendar>
      {
         new Calendar() { Title = "全院行事曆" , Key = CalendarTypes.HLH },
         new Calendar() { Title = "資訊室行事曆" , Key = "IT" },
         new Calendar() { Title = "延伸法庭行事曆" , Key = CalendarTypes.EC }
      };
      foreach (var item in calendars) await AddCalendarIfNotExist(context, item);
      context.SaveChanges();
   }
   static async Task AddCalendarIfNotExist(DefaultContext context, Calendar calendar)
   {
      var dbset = context.Calendars;
      if (dbset.Count() == 0)
      {
         dbset.Add(calendar);
         return;
      }
      var exist = await dbset.FirstOrDefaultAsync(x => x.Key == calendar.Key);
      if (exist == null) dbset.Add(calendar);
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
      var dbset = context.Departments;
      if (dbset.Count() == 0) 
		{
         dbset.Add(department);
         return;
      }
		var exist = await dbset.FirstOrDefaultAsync(x => x.Key == department.Key);
		if (exist == null) dbset.Add(department);
   }
   static async Task SeedTags(DefaultContext context)
   {
      var departments = context.Departments.Where(x => !x.Removed).ToList();
      foreach (var department in departments)
      {
         var tag = new Tag { Key = department.Key, Title = department.Title };
         await AddTagIfNotExist(context, tag);
      } 
      context.SaveChanges();
   }
   static async Task AddTagIfNotExist(DefaultContext context, Tag tag)
   {
      var dbset = context.Tags;
      if (dbset.Count() == 0)
      {
         dbset.Add(tag);
         return;
      }
      var exist = await dbset.FirstOrDefaultAsync(x => x.Key == tag.Key || x.Title == tag.Title);
      if (exist == null) dbset.Add(tag);
   }
   static async Task SeedLocations(DefaultContext context)
   {
      var locations = new List<Location>
      {
         new Location() { Title = "行政大樓", Key = "AB" },
         new Location() { Title = "法庭大樓", Key = "CB" },
         new Location() { Title = "台東庭", Key = "TD" },
      };
      foreach (var item in locations) await AddLocationIfNotExist(context, item);
      context.SaveChanges();
   }
   static async Task AddLocationIfNotExist(DefaultContext context, Location location)
   {
      var dbset = context.Locations;
      if (dbset.Count() == 0)
      {
         dbset.Add(location);
         return;
      }
      var exist = await dbset.FirstOrDefaultAsync(x => x.Title == location.Title);
      if (exist == null) dbset.Add(location);
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
      var dbset = context.JobTitles;
      if (dbset.Count() == 0)
      {
         dbset.Add(jobTitle);
         return;
      }
      var exist = await dbset.FirstOrDefaultAsync(x => x.Title == jobTitle.Title);
		if (exist == null) dbset.Add(jobTitle);
		else exist.Order = jobTitle.Order;
   }
   static async Task SeedCourts(DefaultContext context)
   {
      //遠距詢問FD, 雙向詢問DW, U會議詢問UM
      var courts = new List<Court>
      {
         new Court() { Title = "第一法庭", Key = "C1", Utils = "DW,UM", Order = 0 },
         new Court() { Title = "第二法庭", Key = "C2", Utils = "DW,UM", Order = 1 },
         new Court() { Title = "第三法庭", Key = "C3", Utils = "FD,DW,UM", Order = 2 },
         new Court() { Title = "第四法庭", Key = "C4", Utils = "FD,DW,UM", Order = 3 },

         new Court() { Title = "台東第四法庭", Key = "TDC4", Utils = "FD,DW,UM", Order = 4 },
         new Court() { Title = "台東第五法庭", Key = "TDC5", Utils = "FD,DW,UM", Order = 5 },
         new Court() { Title = "台東第二法庭", Key = "TDC2", Utils = "FD,DW,UM", Order = 6 },
      };
      foreach (var item in courts)
      {
         var location = context.Locations.FirstOrDefault(x => x.Key == item.Key);
         item.LocationId = location!.Id;
         await AddCourtIfNotExist(context, item);
      } 
      context.SaveChanges();
   }
   static async Task AddCourtIfNotExist(DefaultContext context, Court court)
   {
      var dbset = context.Courts;
      if (dbset.Count() == 0)
      {
         dbset.Add(court);
         return;
      }
      var exist = await dbset.FirstOrDefaultAsync(x => x.Key == court.Key);
      if (exist == null) dbset.Add(court);
      else
      {
         exist.Title = court.Title;
         exist.Utils = court.Utils;
         exist.Order = court.Order;
      }
   }
}