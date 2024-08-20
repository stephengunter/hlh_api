using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class HostSpecification : Specification<Host>
{
   public HostSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
