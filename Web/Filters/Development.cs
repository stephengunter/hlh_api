using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters
{
   public class DevelopmentOnlyFilter : IActionFilter
   {
      public void OnActionExecuting(ActionExecutingContext context)
      {
         var env = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>();
         if (env is null || !env.IsDevelopment())
         {
            // If we're not running in development mode, then just return a 404
            context.Result = new NotFoundResult();
         }
      }

      public void OnActionExecuted(ActionExecutedContext context)
      {
         
      }
   }
}