using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Database;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // SQL DB registration (leave for non-archival entities)
        var sqlConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(sqlConnectionString))
        {
            throw new InvalidOperationException("Database connection string not found. Please ensure DefaultConnection is configured in appsettings.");
        }
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));

        return services;
    }

    public static void RunDatabaseMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseMigration");
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            logger.LogError("Database connection string not found for migration");
            throw new InvalidOperationException("Database connection string not found. Please ensure DefaultConnection is configured in appsettings.");
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
