using Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Persistence.Repositories;
using DotNetEnv;

namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
        
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                              ?? configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string not found. Please ensure CONNECTION_STRING is set in .env file or DefaultConnection is configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IStateRepository, StateRepository>();
        services.AddScoped<IBoardMemberRepository, BoardMemberRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }

    public static void RunDatabaseMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseMigration");
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
        
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
                              ?? configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            logger.LogError("Database connection string not found for migration");
            throw new InvalidOperationException("Database connection string not found. Please ensure CONNECTION_STRING is set in .env file or DefaultConnection is configured.");
        }

        if (!DbUpMigrator.ValidateConnection(connectionString, logger))
        {
            throw new InvalidOperationException("Database connection validation failed. Please check your connection string and database availability.");
        }

        var migrationSuccess = DbUpMigrator.MigrateDatabase(connectionString, logger);
        
        if (!migrationSuccess)
        {
            throw new InvalidOperationException("Database migration failed. Application startup aborted.");
        }
    }
}
