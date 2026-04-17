using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Dtos.Chat;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Chat;

public class AskChatEndpoint : EndpointBase
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chat/ask", async (
            ChatRequest request,
            IChatService chatService,
            IAuthenticationService authService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return Results.BadRequest("Message cannot be empty");
            }

            var userIdString = await authService.GetCurrentUserIdAsync();
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Results.Unauthorized();
            }

            request.UserId = userId;

            var response = await chatService.GetResponseAsync(request);
            return Results.Ok(response);
        })
        .WithName("AskChat")
        .WithTags("Chat")
        .RequireAuthorization(Policies.RequireUserRole)
        .Produces<ChatResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
