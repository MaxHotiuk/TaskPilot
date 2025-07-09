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

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.RequireAdminRole, policy =>
                policy.Requirements.Add(new RoleRequirement(Roles.Admin)));
            
            options.AddPolicy(Policies.RequireUserRole, policy =>
                policy.Requirements.Add(new RoleRequirement(Roles.User)));
            
            options.AddPolicy(Policies.RequireBoardMember, policy =>
                policy.Requirements.Add(new BoardMemberRequirement(requireOwner: false, allowMember: true)));
            
            options.AddPolicy(Policies.RequireBoardOwner, policy =>
                policy.Requirements.Add(new BoardOwnerRequirement()));
            
            options.AddPolicy(Policies.RequireBoardMemberOrOwner, policy =>
                policy.Requirements.Add(new BoardMemberOrOwnerRequirement()));
            
            options.AddPolicy(Policies.RequireCommentOwner, policy =>
                policy.Requirements.Add(new CommentOwnerRequirement()));
        });

        services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, BoardMemberAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, BoardOwnerAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, BoardMemberOrOwnerAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, CommentOwnerAuthorizationHandler>();

        services.AddHttpContextAccessor();

        return services;
    }
}
