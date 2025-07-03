using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using WebApi.Authorization;
using Domain.Common.Authorization;

namespace WebApi.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Use the existing AzureAd configuration from appsettings
        var azureAdSection = configuration.GetSection("AzureAd");
        
        if (string.IsNullOrEmpty(azureAdSection["TenantId"]))
        {
            throw new InvalidOperationException("AzureAd:TenantId not found in configuration");
        }
        
        if (string.IsNullOrEmpty(azureAdSection["ClientId"]))
        {
            throw new InvalidOperationException("AzureAd:ClientId not found in configuration");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(azureAdSection);

        // Add authorization with role-based policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.RequireAdminRole, policy =>
                policy.Requirements.Add(new RoleRequirement(Roles.Admin)));
            
            options.AddPolicy(Policies.RequireUserRole, policy =>
                policy.Requirements.Add(new RoleRequirement(Roles.User)));
        });

        // Register authorization handler
        services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

        services.AddHttpContextAccessor();

        return services;
    }
}
