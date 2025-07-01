using Application;
using Infrastructure;
using Persistence;
using Serilog;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

try
{
    Persistence.DependencyInjection.RunDatabaseMigrations(app.Services);
    app.Logger.LogInformation("Database migration completed successfully");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Database migration failed during application startup");
    throw;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.Run();
