using Application;
using Infrastructure;
using Persistence;
using Serilog;
using WebApi.Extensions;
using WebApi.Middlewares;
using Hangfire;
using Hangfire.MemoryStorage;
using Application.Abstractions.Archivation;

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

// Add authentication and authorization
builder.Services.AddAuthentication(builder.Configuration);

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
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// Add Hangfire services with in-memory storage for development
builder.Services.AddHangfire(config =>
    config.UseMemoryStorage()
);
builder.Services.AddHangfireServer();

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

// Hangfire dashboard (optional, remove or secure in production)
app.UseHangfireDashboard("/hangfire");

// Schedule recurring jobs after Hangfire is initialized
using (var scope = app.Services.CreateScope())
{
    var scheduler = scope.ServiceProvider.GetService<IArchivalJobScheduler>();
    scheduler?.ScheduleRecurringJobs();
}

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

// Add authentication middleware
app.UseAuthentication();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseAuthorization();

// Map all endpoints
app.MapEndpoints();

app.Run();
