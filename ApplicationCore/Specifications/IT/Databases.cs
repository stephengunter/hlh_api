using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class DatabaseSpecification : Specification<Database>
{
   public DatabaseSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
