using Ardalis.Specification;
using ApplicationCore.Models.Files;

namespace ApplicationCore.Specifications;
public class JudgebookFilesSpecification : Specification<JudgebookFile>
{
   public JudgebookFilesSpecification()
   {
      Query.Where(item => !item.Removed);
   }

   public JudgebookFilesSpecification(JudgebookFile model)
   {
      Query.Where(item => !item.Removed && item.Year == model.Year && item.Category == model.Category && item.Num == model.Num);
   }

}

