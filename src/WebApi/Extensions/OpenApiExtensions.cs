using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace WebApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TaskPilot API",
                Version = "v1",
                Description = "TaskPilot — Your Mission Control for Task Management",
                Contact = new OpenApiContact
                {
                    Name = "TaskPilot Team"
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskPilot API v1");
            c.RoutePrefix = "swagger";
        });

        return app;
    }

    public static WebApplication MapOpenApi(this WebApplication app)
    {
        app.UseOpenApi();
        return app;
    }
}
