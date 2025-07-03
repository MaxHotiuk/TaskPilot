using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Authorization;
using Domain.Common.Authorization;

namespace WebApi.Authorization;

public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly IAuthenticationService _authenticationService;

    public RoleAuthorizationHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        RoleRequirement requirement)
    {
        if (!await _authenticationService.IsUserAuthenticatedAsync())
        {
            context.Fail();
            return;
        }

        var userRole = await _authenticationService.GetCurrentUserRoleAsync();
        
        if (string.IsNullOrEmpty(userRole))
        {
            context.Fail();
            return;
        }

        if (userRole == Roles.Admin)
        {
            context.Succeed(requirement);
            return;
        }

        if (userRole == requirement.Role)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}
