using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace csharp.Policies
{
  // Handler for SamePersonOrAdmin authorization requirement and policy
  public class SamePersonOrAdminHandler : AuthorizationHandler<SamePersonOrAdminRequirement, GetUserDto>
  {
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SamePersonOrAdminRequirement requirement, GetUserDto resource)
    {
      if (
        context.User.HasClaim(ClaimTypes.Actor, resource.Id.ToString())
        || context.User.HasClaim(ClaimTypes.Role, "Admin")
      )
      {
        context.Succeed(requirement);
      }

      return Task.CompletedTask;
    }
  }
}