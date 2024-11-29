using ApplicationCore.Models;
using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class DatabaseSpecification : Specification<Database>
{
   public DatabaseSpecification(ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!) Query.Include(item);
      }
      Query.Where(item => !item.Removed);
   }
   public DatabaseSpecification(int id, ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!) Query.Include(item);
      }
      Query.Where(item => !item.Removed && item.Id == id);
   }
   
}