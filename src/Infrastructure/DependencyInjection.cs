using Application.Abstractions.Authentication;
using Infrastructure.Services.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Application.Abstractions.Storage;
using Infrastructure.Services.Storage;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IAvatarService, AvatarService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        return services;
    }
}