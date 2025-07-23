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
        // SQL DB registration (leave for non-archival entities)
        var sqlConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(sqlConnectionString))
        {
            throw new InvalidOperationException("Database connection string not found. Please ensure DefaultConnection is configured in appsettings.");
        }
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<IBoardMemberRepository, BoardMemberRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        services.AddScoped<ICosmosArchivalJobRepository, CosmosArchivalJobRepository>();

        return services;
    }
}
