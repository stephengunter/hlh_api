using ApplicationCore.Models;
using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class ServerSpecification : Specification<Server>
{
   public ServerSpecification(string include = "")
   {
      foreach (var item in FetchIncludes(include))
      {
         Query.Include(item);
      }
      Query.Where(item => !item.Removed);
   }
   public ServerSpecification(int id,string include = "")
   {
      foreach (var item in FetchIncludes(include))
      {
         Query.Include(item);
      }
      Query.Where(item => !item.Removed && item.Id == id);
   }
   static ICollection<string> FetchIncludes(string include)
   {
      var result = new List<string>();
      foreach (var item in include.SplitToList())
      {
         if (item.EqualTo(nameof(Server.Host))) result.Add(nameof(Server.Host));
      }
      return result;

   }
}