using Application.Abstractions.Authentication;
using Infrastructure.Services.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Application.Abstractions.Storage;
using Infrastructure.Services.Storage;
using Application.Abstractions.Archivation;
using Infrastructure.Services.Archivation;
using Infrastructure.BackgroundJobs;
using Azure.Messaging.ServiceBus;
using Application.Abstractions.Messaging;
using Infrastructure.Services;

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
        services.AddScoped<IArchivalService, ArchivalService>();
        services.AddScoped<IArchivalBackgroundJob, ArchivalBackgroundJob>();
        services.AddScoped<IArchivalJobScheduler, ArchivalJobScheduler>();

        services.AddScoped<IBoardNotifier, BoardNotifier>();

        services.AddSingleton(provider =>
        {
            var connectionString = configuration.GetConnectionString("ServiceBus") 
                ?? throw new InvalidOperationException("ServiceBus connection string is not configured");
            return new ServiceBusClient(connectionString);
        });
        return services;
    }
}