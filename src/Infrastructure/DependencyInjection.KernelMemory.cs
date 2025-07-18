using Application.Abstractions.Messaging;
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
            var memory = new KernelMemoryBuilder()
                .WithAzureOpenAITextGeneration(new AzureOpenAIConfig
                {
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIKey = configuration["AzureOpenAI:ApiKey"] ?? throw new ArgumentNullException("AzureOpenAI:ApiKey"),
                    Endpoint = configuration["AzureOpenAI:Endpoint"] ?? throw new ArgumentNullException("AzureOpenAI:Endpoint"),
                    Deployment = configuration["AzureOpenAI:DeploymentName"] ?? throw new ArgumentNullException("AzureOpenAI:DeploymentName")
                })
                .WithAzureOpenAITextEmbeddingGeneration(new AzureOpenAIConfig
                {
                    Auth = AzureOpenAIConfig.AuthTypes.APIKey,
                    APIKey = configuration["KernelMemory:EmbeddingGenerator:ApiKey"] ?? throw new ArgumentNullException("KernelMemory:EmbeddingGenerator:ApiKey"),
                    Endpoint = configuration["KernelMemory:EmbeddingGenerator:Endpoint"] ?? throw new ArgumentNullException("KernelMemory:EmbeddingGenerator:Endpoint"),
                    Deployment = configuration["KernelMemory:EmbeddingGenerator:DeploymentName"] ?? throw new ArgumentNullException("KernelMemory:EmbeddingGenerator:DeploymentName")
                })
                .WithAzureAISearchMemoryDb(new AzureAISearchConfig
                {
                    Auth = AzureAISearchConfig.AuthTypes.APIKey,
                    APIKey = configuration["AzureAISearch:ApiKey"] ?? throw new ArgumentNullException("AzureAISearch:APIKey"),
                    Endpoint = configuration["AzureAISearch:Endpoint"] ?? throw new ArgumentNullException("AzureAISearch:Endpoint")
                })
                .Build();
            return memory;
        });

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IFAQDataService, FAQDataService>();
        return services;
    }
}
