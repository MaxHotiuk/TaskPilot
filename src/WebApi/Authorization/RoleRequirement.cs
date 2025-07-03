using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization;

public class RoleRequirement : IAuthorizationRequirement
{
    public string Role { get; }

    public RoleRequirement(string role)
    {
        Role = role;
    }
}
