using Microsoft.AspNetCore.Authorization;

namespace WebApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
    public string? Policy { get; set; }
    public string? Roles { get; set; }
    public string? AuthenticationSchemes { get; set; }
    
    public AuthorizeAttribute()
    {
    }
    
    public AuthorizeAttribute(string policy)
    {
        Policy = policy;
    }
}
