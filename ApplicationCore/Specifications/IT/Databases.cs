using ApplicationCore.Models;
using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class DatabaseSpecification : Specification<Database>
{
   public DatabaseSpecification(string include = "")
   {
      foreach (var item in FetchIncludes(include))
      {
         Query.Include(item);
      }
      Query.Where(item => !item.Removed);
   }
   public DatabaseSpecification(int id, string include = "")
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
         if (item.EqualTo(nameof(Database.Server))) result.Add(nameof(Database.Server));
      }
      return result;

   }
}