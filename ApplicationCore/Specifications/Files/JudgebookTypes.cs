using Ardalis.Specification;
using ApplicationCore.Models.Files;

namespace ApplicationCore.Specifications;
public class JudgebookTypesSpecification : Specification<JudgebookType>
{
   public JudgebookTypesSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}

