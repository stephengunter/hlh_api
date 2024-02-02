using ApplicationCore.Consts;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ApplicationCore.Helpers;

public static class DbContextHelpers
{
	public static List<string> UserIdsInRole(this DefaultContext context, IdentityRole role)
      => context.UserRoles.Where(x => x.RoleId == role.Id).Select(b => b.UserId).Distinct().ToList();


}