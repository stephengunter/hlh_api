using Ardalis.Specification;
using ApplicationCore.Models.Files;
using ApplicationCore.Models;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class JudgebookFilesSpecification : Specification<JudgebookFile>
{
   public JudgebookFilesSpecification(string include = "")
   {
      if (include.EqualTo("type")) Query.Include(item => item.Type).Where(item => !item.Removed);
      else Query.Where(item => !item.Removed);
   }

   public JudgebookFilesSpecification(JudgebookType type, string include = "")
   {
      if (include.EqualTo("type")) Query.Include(item => item.Type).Where(item => !item.Removed && item.TypeId == type.Id);
      else Query.Where(item => !item.Removed && item.TypeId == type.Id);
   }
   public JudgebookFilesSpecification(int id, string include = "")
   {
      if (include.EqualTo("type")) Query.Include(item => item.Type).Where(item => !item.Removed && item.Id == id);
      else Query.Where(item => !item.Removed && item.Id == id);
   }
}

