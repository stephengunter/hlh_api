using ApplicationCore.Views;

namespace Web.Models;

public class DepartmentsAdminView
{
   public DepartmentsAdminView(List<DepartmentViewModel> departments, Dictionary<string, string> keys)
   {
      Departments = departments;
      Keys = keys;
   }

   public List<DepartmentViewModel> Departments { get; }
   public Dictionary<string, string> Keys { get; }

}