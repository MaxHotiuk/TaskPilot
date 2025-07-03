using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

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

        services.AddAuthorization();

        services.AddHttpContextAccessor();

        return services;
    }
}
