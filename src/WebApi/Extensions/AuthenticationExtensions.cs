using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using DotNetEnv;

namespace WebApi.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
        
        var azureAdConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureAd:Instance"] = Env.GetString("AZURE_AD_INSTANCE") ?? "https://login.microsoftonline.com/",
                ["AzureAd:TenantId"] = Env.GetString("AZURE_AD_TENANT_ID") ?? throw new InvalidOperationException("AZURE_AD_TENANT_ID not found in environment variables"),
                ["AzureAd:ClientId"] = Env.GetString("AZURE_AD_CLIENT_ID") ?? throw new InvalidOperationException("AZURE_AD_CLIENT_ID not found in environment variables"),
                ["AzureAd:Audience"] = Env.GetString("AZURE_AD_AUDIENCE") ?? throw new InvalidOperationException("AZURE_AD_AUDIENCE not found in environment variables")
            })
            .Build();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(azureAdConfig.GetSection("AzureAd"));

        services.AddAuthorization();

        services.AddHttpContextAccessor();

        return services;
    }
}
