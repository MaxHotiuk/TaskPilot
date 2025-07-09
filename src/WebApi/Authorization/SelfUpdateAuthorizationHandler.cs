using Application.Abstractions.Authentication;
using Domain.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApi.Authorization;

public class SelfUpdateRequirement : IAuthorizationRequirement { }

public class SelfUpdateAuthorizationHandler : AuthorizationHandler<SelfUpdateRequirement>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<SelfUpdateAuthorizationHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SelfUpdateAuthorizationHandler(IAuthenticationService authenticationService, ILogger<SelfUpdateAuthorizationHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _authenticationService = authenticationService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SelfUpdateRequirement requirement)
    {
        _logger.LogInformation("SelfUpdateRequirement authorization check started");

        if (!await _authenticationService.IsUserAuthenticatedAsync())
        {
            _logger.LogWarning("User is not authenticated");
            context.Fail();
            return;
        }

        var userRole = await _authenticationService.GetCurrentUserRoleAsync();
        _logger.LogInformation("User role: {UserRole}", userRole);

        if (userRole == Roles.Admin)
        {
            _logger.LogInformation("User is admin, granting access");
            context.Succeed(requirement);
            return;
        }

        var currentUserId = await _authenticationService.GetCurrentUserIdAsync();
        _logger.LogInformation("Current user ID: {UserId}", currentUserId);

        if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
        {
            _logger.LogWarning("Could not parse user ID: {UserId}", currentUserId);
            context.Fail();
            return;
        }

        var requestUserId = GetUserIdFromContext(context);

        if (requestUserId != null && requestUserId.Value == userId)
        {
            _logger.LogInformation("User is updating their own profile, granting access");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("User is not authorized to update this profile");
            context.Fail();
        }
    }

    private Guid? GetUserIdFromContext(AuthorizationHandlerContext context)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        if (httpContext.Request.RouteValues.TryGetValue("userid", out var userIdValue) &&
            Guid.TryParse(userIdValue?.ToString(), out var userId))
        {
            return userId;
        }

        if (httpContext.Request.RouteValues.TryGetValue("id", out var idValue) &&
            Guid.TryParse(idValue?.ToString(), out var id))
        {
            return id;
        }
        
        return null;
    }
}
