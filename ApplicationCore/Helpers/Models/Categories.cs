using ApplicationCore.Consts;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Entities;
using Infrastructure.Views;

namespace ApplicationCore.Helpers;
public static class CategoriesHelpers
{
   //public static void LoadSubItems<T>(this BaseCategory<T> model, IEnumerable<BaseCategory<T>> allCategories)
   //   where T : BaseCategory<T>
   //{
   //   model.SubItems = allCategories.Where(item => item.ParentId == model.Id).Cast(T);
   //   var subItems = allCategories.Where(item => item.ParentId == model.Id);
   //   if (subItems.HasItems()) model.SubItems = subItems;//OrderBy(item => item.Order).Cast(T);
      
   //   foreach (var item in model.SubItems) item.LoadSubItems(subItems);
   //}

   public static IEnumerable<BaseCategory<T>> RootCategories<T>(this IEnumerable<BaseCategory<T>> allCategories)
     where T : BaseCategory<T>
      => allCategories.Where(item => item.IsRootItem);

   

   
   

}
