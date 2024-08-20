using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class SystemAppSpecification : Specification<SystemApp>
{
   public SystemAppSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
