using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Consts;

namespace ApplicationCore.Authorization;

public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
{
   protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
   {
      if (requirement.Permission == Permissions.Admin)
      {
         if (context.User.IsBoss() || context.User.IsDev())
         {
            context.Succeed(requirement);
            return Task.CompletedTask;
         }
      }

      context.Fail();
      return Task.CompletedTask;
   }
}
