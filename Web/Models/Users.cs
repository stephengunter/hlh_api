using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Paging;
using Infrastructure.Views;
using Microsoft.AspNetCore.Identity;
using System;

namespace Web.Models;

public class UsersAdminRequest
{
   public UsersAdminRequest(bool active, int? department, string? role, string? keyword, int page = 1, int pageSize = 10)
   {
      Active = active;
      Department = department;
      Role = role;
      Keyword = keyword;
      Page = page < 1 ? 1 : page;
      PageSize = pageSize;
   }
   public bool Active { get; set; }
   public int? Department { get; set; }
   public string? Role { get; set; }
   public string? Keyword { get; set; }
   public int Page { get; set; } 
   public int PageSize { get; set; }
}
public class UsersAdminModel
{
   public UsersAdminModel(UsersAdminRequest request, ICollection<RoleViewModel> roles, ICollection<DepartmentViewModel> departments)
   {
      Request = request;
      Roles = roles;
      Departments = departments;
   }

   public UsersAdminRequest Request { get; set; }

   public ICollection<RoleViewModel> Roles{ get; set; } = new List<RoleViewModel>();
   public ICollection<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();

}

public class UsersImportRequest
{
   public List<IFormFile> Files { get; set; } = new List<IFormFile>();
}