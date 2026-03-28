using Application.Abstractions.Authentication;
using Infrastructure.Services.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Database;
using Application.Abstractions.Storage;
using Infrastructure.Services.Storage;
using Application.Abstractions.Archivation;
using Infrastructure.Services.Archivation;
using Infrastructure.BackgroundJobs;
using Azure.Messaging.ServiceBus;
using Application.Abstractions.Messaging;
using Infrastructure.Services;
using Infrastructure.Services.Meetings;
using Application.Abstractions.Meetings;
using System.Net.Http.Headers;
using Infrastructure.Services.Email;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddDatabase(configuration);
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IAvatarService, AvatarService>();
        services.AddScoped<IChatAvatarService, ChatAvatarService>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        services.AddScoped<IArchivalService, ArchivalService>();
        services.AddScoped<IArchivalBackgroundJob, ArchivalBackgroundJob>();
        services.AddScoped<IArchivalJobScheduler, ArchivalJobScheduler>();

        services.AddScoped<IBoardNotifier, BoardNotifier>();
        services.AddScoped<INotificationNotifier, NotificationNotifier>();
        services.AddScoped<IChatNotifier, ChatNotifier>();

        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.AddScoped<IEmailService, EmailService>();

        services.Configure<DailyOptions>(configuration.GetSection("Daily"));
        services.AddHttpClient<IDailyRoomService, DailyRoomService>()
            .ConfigureHttpClient((provider, client) =>
            {
                var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<DailyOptions>>().Value;
                client.BaseAddress = new Uri(options.ApiBaseUrl);
                if (!string.IsNullOrWhiteSpace(options.ApiKey))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
                }
            });

        services.AddSingleton(provider =>
        {
            var connectionString = configuration.GetConnectionString("ServiceBus") 
                ?? throw new InvalidOperationException("ServiceBus connection string is not configured");
            return new ServiceBusClient(connectionString);
        });
        return services;
    }
}