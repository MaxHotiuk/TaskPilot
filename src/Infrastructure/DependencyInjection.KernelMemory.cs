using Application.Abstractions.Messaging;
using Infrastructure.BackgroundJobs;
using Infrastructure.Services.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;

namespace Infrastructure;

public static class DependencyInjectionKernelMemory
{
    public static IServiceCollection AddKernelMemory(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IKernelMemory>(provider =>
        {
            var apiKey = configuration["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Gemini:ApiKey is not configured.");

            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var embeddingGenerator = new GeminiEmbeddingGenerator(
                httpClient: httpClientFactory.CreateClient("Gemini"),
                apiKey: apiKey);

            var memory = new KernelMemoryBuilder()
                .WithoutTextGenerator()
                .WithCustomEmbeddingGenerator(embeddingGenerator)
                .WithSqlServerMemoryDb(
                    configuration.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured."))
                .WithAzureBlobsDocumentStorage(new AzureBlobsConfig
                {
                    Auth = AzureBlobsConfig.AuthTypes.ConnectionString,
                    ConnectionString = configuration["AzureBlob:ConnectionString"]
                        ?? throw new InvalidOperationException("AzureBlob:ConnectionString is not configured."),
                    Container = configuration["AzureBlob:KernelMemoryContainerName"] ?? "kernel-memory"
                })
                .Build();
            return memory;
        });

        services.AddHttpClient("Gemini");

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IFAQDataService, FAQDataService>();
        services.AddScoped<IAiContextSyncService, AiContextSyncService>();
        services.AddScoped<IAiSyncEnqueuer, AiSyncEnqueuer>();
        return services;
    }
}

