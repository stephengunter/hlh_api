using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ApplicationCore.Models;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.DataAccess;

public static class SeedData
{
	
	static string DevRoleName = AppRoles.Dev.ToString();
	static string BossRoleName = AppRoles.Boss.ToString();

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
		using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
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
					SecurityStamp = Guid.NewGuid().ToString()
				};
				await CreateUserIfNotExist(userManager, user, new List<string>() { DevRoleName });
			}
		}

      await SeedDepartments(context);
		
		Console.WriteLine("Done seeding database.");
	}

	static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
	{
		var roles = new List<string> { DevRoleName, BossRoleName };
		foreach (var item in roles) await AddRoleIfNotExist(roleManager, item);
	}
	static async Task AddRoleIfNotExist(RoleManager<IdentityRole> roleManager, string roleName)
	{
		var role = await roleManager.FindByNameAsync(roleName);
		if (role == null) await roleManager.CreateAsync(new IdentityRole { Name = roleName });
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

   static async Task SeedDepartments(DefaultContext context)
   {
		var departments = new List<Department>
		{
			new Department() { Title = "司法院", Key = "JUD" },
         new Department() { Title = "花蓮高分院", Key = "HLH" },
      };
      foreach (var item in departments) await AddDepartmentsIfNotExist(context, item);
		context.SaveChanges();
   }
   static async Task AddDepartmentsIfNotExist(DefaultContext context, Department department)
   {
		var exist = await context.Departments.FirstOrDefaultAsync(x => x.Key == department.Key);

		if (exist == null) context.Departments.Add(department);
   }

}