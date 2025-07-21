using DbUp;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Database;

public static class DbUpMigrator
{
    public static bool MigrateDatabase(string connectionString, ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting database migration...");

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), name => name.Contains(".Migrations."))
                .LogToAutodetectedLog()
                .Build();

            if (!upgrader.IsUpgradeRequired())
            {
                logger.LogInformation("Database is already up to date. No migration required.");
                return true;
            }

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                logger.LogError(result.Error, "Database migration failed: {ErrorMessage}", result.Error?.Message);
                return false;
            }

            logger.LogInformation("Database migration completed successfully. Scripts executed: {ScriptCount}", 
                result.Scripts?.Count() ?? 0);

            if (result.Scripts?.Any() == true)
            {
                logger.LogDebug("Executed migration scripts:");
                foreach (var script in result.Scripts)
                {
                    logger.LogDebug("- {ScriptName}", script.Name);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during database migration");
            return false;
        }
    }

    public static IEnumerable<string> GetPendingMigrations(string connectionString)
    {
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), name => name.Contains(".Migrations."))
            .Build();

        return upgrader.GetScriptsToExecute().Select(s => s.Name);
    }

    public static bool ValidateConnection(string connectionString, ILogger logger)
    {
        try
        {
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .Build();

            upgrader.GetExecutedScripts();
            
            logger.LogInformation("Database connection validated successfully");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database connection validation failed");
            return false;
        }
    }
}
