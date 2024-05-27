using Ardalis.Specification;
using ApplicationCore.Models.Files;
using ApplicationCore.Models;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class JudgebookFilesSpecification : Specification<JudgebookFile>
{
   static bool IncludeType(string include) => include.EqualTo("type");
   public JudgebookFilesSpecification(string include = "")
   {
      if (IncludeType(include))
      {
         Query.Include(item => item.Type).Where(item => !item.Removed);
      }
      else Query.Where(item => !item.Removed);
   }

   public JudgebookFilesSpecification(JudgebookType type, string include = "")
   {
      if (IncludeType(include))
      {
         Query.Include(item => item.Type).Where(item => !item.Removed && item.TypeId == type.Id);
      }
      else Query.Where(item => !item.Removed && item.TypeId == type.Id);
   }
   public JudgebookFilesSpecification(int id, string include = "")
   {
      if (IncludeType(include))
      {
         Query.Include(item => item.Type).Where(item => !item.Removed && item.Id == id);
      }
      else Query.Where(item => !item.Removed && item.Id == id);
   }
   public JudgebookFilesSpecification(IEnumerable<int> ids, string include = "")
   {
      if (IncludeType(include)) 
      {
         Query.Include(item => item.Type).Where(item => !item.Removed && ids.Contains(item.Id));
      } 
      else Query.Where(item => !item.Removed && ids.Contains(item.Id));
   }
}

public class JudgebookFileSameSaceSpecification : Specification<JudgebookFile>
{
   static bool IncludeType(string include) => include.EqualTo("type");
   public JudgebookFileSameSaceSpecification(JudgebookFile model, string include = "")
   {
      if (IncludeType(include))
      {
         Query.Include(item => item.Type).Where(entry => entry.Id != model.Id && !entry.Removed
            && entry.TypeId == model.TypeId && entry.CourtType == model.CourtType && entry.Year == model.Year
               && entry.Category == model.Category && entry.Num == model.Num);
      } 
      else Query.Where(entry => entry.Id != model.Id && !entry.Removed
            && entry.TypeId == model.TypeId && entry.CourtType == model.CourtType && entry.Year == model.Year
               && entry.Category == model.Category && entry.Num == model.Num);
   }
}

