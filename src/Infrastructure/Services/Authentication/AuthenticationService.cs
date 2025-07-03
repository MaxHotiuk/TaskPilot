using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<string?> GetCurrentUserIdAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Task.FromResult(userId);
    }

    public Task<string?> GetCurrentUserEmailAsync()
    {
        var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ??
                   _httpContextAccessor.HttpContext?.User?.FindFirst("preferred_username")?.Value;
        return Task.FromResult(email);
    }

    public Task<bool> IsUserAuthenticatedAsync()
    {
        var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        return Task.FromResult(isAuthenticated);
    }

    public Task<string?> GetCurrentUserEntraIdAsync()
    {
        var entraId = _httpContextAccessor.HttpContext?.User?.FindFirst("oid")?.Value ??
                     _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Task.FromResult(entraId);
    }
}
