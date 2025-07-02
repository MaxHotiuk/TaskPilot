using Application;
using Infrastructure;
using Persistence;
using Serilog;
using DotNetEnv;
using WebApi.Extensions;
using WebApi.Middlewares;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Register OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TaskPilot API",
        Version = "v1",
        Description = "TaskPilot — Your Mission Control for Task Management"
    });
});

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

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure Swagger/OpenAPI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskPilot API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAll");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

// Map all endpoints
app.MapEndpoints();

app.Run();
