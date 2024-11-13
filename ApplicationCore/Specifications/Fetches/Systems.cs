using ApplicationCore.Models.Fetches;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Fetches;
public class FetchSystemSpecification : Specification<FetchesSystem>
{
   public FetchSystemSpecification(string title)
   {
      Query.Where(x => x.Title == title);
   }
}
