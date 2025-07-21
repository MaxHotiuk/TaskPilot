using Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // CosmosDB registration for archivation info
        var cosmosConnectionString = configuration.GetConnectionString("CosmosDb");
        var cosmosDbName = configuration.GetSection("CosmosDb:DatabaseName").Value;
        if (string.IsNullOrEmpty(cosmosConnectionString) || string.IsNullOrEmpty(cosmosDbName))
        {
            throw new InvalidOperationException("CosmosDb connection string or database name not found. Please ensure CosmosDb is configured in appsettings.");
        }
        services.AddDbContext<Cosmos.CosmosDbContext>(options =>
            options.UseCosmos(cosmosConnectionString, cosmosDbName));
        services.AddScoped<ICosmosArchivalJobRepository, Cosmos.CosmosArchivalJobRepository>();

        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<IBoardMemberRepository, BoardMemberRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }
}
