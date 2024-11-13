using ApplicationCore.Models;
using ApplicationCore.Views;
using Infrastructure.Helpers;

namespace Web.Models;

public class UserLabels
{
   public string UserName { get; } = "UserName";
   public string Email { get; } = "Email";
   public string Name { get; } = "名稱";
   public string PhoneNumber { get; } = "手機號碼";
   public string Roles { get; } = "角色";
   public string Active { get; } = "狀態";
   public string CreatedAt { get; } = BaseLabels.CreatedAt;
   public string LastUpdated { get; } = BaseLabels.LastUpdated;

   public string Department { get; } = "部門";

   public ProfilesLabels Profiles { get; } = new ProfilesLabels();
}
public class ProfilesLabels
{
   public string Name { get; } = "姓名";
   public string Ps { get; } = "備註";
   public string CreatedAt { get; } = BaseLabels.CreatedAt;
   public string LastUpdated { get; } = BaseLabels.LastUpdated;
}
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
   public UsersAdminModel(UsersAdminRequest request, ICollection<RoleViewModel> roles, ICollection<DepartmentViewModel> departments, DepartmentViewModel rootDepartment)
   {
      Request = request;
      Roles = roles;
      Departments = departments;
      RootDepartment = rootDepartment;
   }
   public UserLabels Labels { get; set; } = new UserLabels();
   public DepartmentViewModel RootDepartment { get; set; }
   public UsersAdminRequest Request { get; set; }

   public ICollection<RoleViewModel> Roles{ get; set; } = new List<RoleViewModel>();
   public ICollection<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();

}

public abstract class BaseUserForm
{
   public string? Id { get; set; }
   public string? UserName { get; set; }
   public string? Name { get; set; }

   public string? Email { get; set; }

   public string? PhoneNumber { get; set; }

   public bool Active { get; set; }
   public ICollection<string> Roles { get; set; } = new List<string>();

   public void SetRoles(ICollection<Role> roles)
   {
      if (roles.HasItems()) Roles = roles!.Select(x => x.Name!).ToList();
      else Roles = new List<string>();
   }

}
public class UserCreateForm : BaseUserForm
{
   
}

public class UserEditForm : BaseUserForm
{
   
}

public class UsersImportRequest
{
   public List<IFormFile> Files { get; set; } = new List<IFormFile>();
}

public class UsersUpDownRequest
{
   public bool Up { get; set; }
   public List<string> Ids { get; set; } = new List<string>();
}