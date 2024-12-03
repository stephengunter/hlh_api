using ApplicationCore.Models;
using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class DbBackupPlanSpecification : Specification<DbBackupPlan>
{
   public DbBackupPlanSpecification(Database db, ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!) Query.Include(item);
      }
      Query.Where(item => !item.Removed && item.DatabaseId == db.Id);
   }
}