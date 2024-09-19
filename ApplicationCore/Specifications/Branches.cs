using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class BranchSpecification : Specification<Branch>
{
   public BranchSpecification()
   {
      Query.Where(item => !item.Removed);
   }
   public BranchSpecification(string title)
   {
      Query.Where(item => !item.Removed && item.Title == title);
   }
}
