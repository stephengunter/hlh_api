using ApplicationCore.Models;
using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class DbBackupPlanSpecification : Specification<DbBackupPlan>
{
   public DbBackupPlanSpecification(Database db)
   {
      Query.Where(item => !item.Removed && item.DatabaseId == db.Id);
   }
}