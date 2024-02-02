using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Views;
using Microsoft.AspNetCore.Identity;
using System;

namespace Web.Models;

public class UsersAdminRequest
{
   public UsersAdminRequest(bool active, string? role, string? keyword, int page = 1, int pageSize = 10)
   {
      Active = active;
      Role = role;
      Keyword = keyword;
      Page = page;
      PageSize = pageSize;
   }
   public bool Active { get; set; }
   public string? Role { get; set; }
   public string? Keyword { get; set; }
   public int Page { get; set; } 
   public int PageSize { get; set; }
}
public class UsersAdminModel
{
   public UsersAdminModel(UsersAdminRequest request)
   {
      Request = request;
   }

   public UsersAdminRequest Request { get; set; }

   public ICollection<BaseOption<string>> RolesOptions { get; set; } = new List<BaseOption<string>>();

   public PagedList<User, UserViewModel>? PagedList { get; set; }
   
   public void LoadRolesOptions(IEnumerable<IdentityRole> roles)
      => RolesOptions = roles.Select(x => x.ToOption()).ToList();
}