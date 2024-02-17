using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Helpers;
using ApplicationCore.Exceptions;
using ApplicationCore.Consts;
using System.Data;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Specifications;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApplicationCore.Services;

public interface IUsersService
{
   #region Fetch
   Task<IEnumerable<User>> FetchAsync(Role? role);
	IEnumerable<Role> FetchRoles();
	#endregion

	#region Find
	Task<User?> FindByIdAsync(string id);
	Task<User?> FindByEmailAsync(string email);
   Task<User?> FindByUsernameAsync(string username);
   User? FindByPhone(string phone);
   Task <Role?> FindRoleAsync(string name);
   #endregion

   #region Get
   Task<User?> GetByIdAsync(string id);
   #endregion

   #region Store
   Task<User> CreateAsync(User user);
	Task UpdateAsync(User user);

	#endregion

	#region Get
	Task<IList<string>> GetRolesAsync(User user);
	IEnumerable<Role> GetRolesByUserId(string userId);

	#endregion

	#region Check
	Task<bool> IsAdminAsync(User user);
   Task<bool> HasPasswordAsync(User user);
   Task<bool> CheckPasswordAsync(User user, string password);
   #endregion
}

public class UsersService : IUsersService
{
	DefaultContext _context;
	private readonly UserManager<User> _userManager;
	private readonly RoleManager<Role> _roleManager;
   private readonly IDefaultRepository<User> _usersRepository;

   public UsersService(DefaultContext context, UserManager<User> userManager, RoleManager<Role> roleManager,
      IDefaultRepository<User> usersRepository)
   {
		_context = context;
		_userManager = userManager;
		_roleManager = roleManager;
      _usersRepository = usersRepository;
   }
	string DevRoleName = AppRoles.Dev.ToString();
	string BossRoleName = AppRoles.Boss.ToString();

   #region Fetch
   public async Task<IEnumerable<User>> FetchAsync(Role? role)
	{
		var users = await _usersRepository.ListAsync(new UsersSpecification());
		if (role is null) return users;

		return FetchByRole(users, role);
   }

   public IEnumerable<Role> FetchRoles() => _roleManager.Roles.ToList();
	#endregion

	#region Find
	public async Task<User?> FindByIdAsync(string id) => await _userManager.FindByIdAsync(id);
	public async Task<User?> FindByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);
   public async Task<User?> FindByUsernameAsync(string username) => await _userManager.FindByNameAsync(username);
   public User? FindByPhone(string phone) => _userManager.Users.FirstOrDefault(x => x.PhoneNumber == phone);
   public async Task<Role?> FindRoleAsync(string name) => await _roleManager.FindByNameAsync(name);
	#endregion


	#region Get
	public async Task<User?> GetByIdAsync(string id)
       => await _usersRepository.FirstOrDefaultAsync(new UsersSpecification(id));
   #endregion

   #region Store
   public async Task<User> CreateAsync(User user)
	{
		var result = await _userManager.CreateAsync(user);
		if (result.Succeeded) return user;

		var error = result.Errors.FirstOrDefault();
		string msg = $"{error!.Code} : {error!.Description}" ?? string.Empty;

		throw new CreateUserException(user, msg);
	}

	public async Task UpdateAsync(User user)
	{
		var result = await _userManager.UpdateAsync(user);
		if(!result.Succeeded)
		{
			var error = result.Errors.FirstOrDefault();
			string msg = $"{error!.Code} : {error!.Description}" ?? string.Empty;

			throw new UpdateUserException(user, msg);
		}
	}

	#endregion

	#region Get
	public async Task<IList<string>> GetRolesAsync(User user) => await _userManager.GetRolesAsync(user);

	public IEnumerable<Role> GetRolesByUserId(string userId)
	{
		var userRoles = _context.UserRoles.Where(x => x.UserId == userId);
		var roleIds = userRoles.Select(ur => ur.RoleId);

		return _roleManager.Roles.Where(r => roleIds.Contains(r.Id));
	}

	#endregion
	
	#region Check
	public async Task<bool> IsAdminAsync(User user)
	{
		var roles = await GetRolesAsync(user);
		if (roles.IsNullOrEmpty()) return false;

		var match = roles.Where(r => r.Equals(DevRoleName) || r.Equals(BossRoleName)).FirstOrDefault();

		return match != null;
	}
   public async Task<bool> HasPasswordAsync(User user)
      => await _userManager.HasPasswordAsync(user);

   public async Task<bool> CheckPasswordAsync(User user, string password)
      => await _userManager.CheckPasswordAsync(user, password);
	#endregion

	#region Helper
	IEnumerable<User> FetchByRole(IEnumerable<User> users, Role role)
	{
      var userIdsInRole = _context.UserRoles.Where(x => x.RoleId == role.Id).Select(b => b.UserId).Distinct().ToList();
      return users.Where(user => userIdsInRole.Contains(user.Id));
   }
   #endregion








}
