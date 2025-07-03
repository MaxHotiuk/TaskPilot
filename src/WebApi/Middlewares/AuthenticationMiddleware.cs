using Application.Abstractions.Authentication;
using Application.Commands.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WebApi.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IMediator mediator, IAuthenticationService authService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                var entraId = context.User.FindFirst("oid")?.Value ?? 
                             context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value ??
                           context.User.FindFirst("preferred_username")?.Value;
                
                var name = context.User.FindFirst(ClaimTypes.Name)?.Value ??
                          context.User.FindFirst("name")?.Value ??
                          email?.Split('@')[0];

                if (!string.IsNullOrEmpty(entraId) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(name))
                {
                    var command = new AuthenticateUserCommand(entraId, email, name);
                    await mediator.Send(command);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user authentication process");
            }
        }

        await _next(context);
    }
}
