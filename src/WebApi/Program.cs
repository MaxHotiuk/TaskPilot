using Application;
using Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

builder.Services
    .AddApplication()
    .AddInfrastructure();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.Run();
