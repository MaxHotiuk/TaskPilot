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
            IChatService chatService) =>
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return Results.BadRequest("Message cannot be empty");
            }
            var response = await chatService.GetResponseAsync(request);
            return Results.Ok(response);
        })
        .WithName("AskChat")
        .WithTags("Chat")
        .RequireAuthorization(Policies.RequireUserRole)
        .Produces<ChatResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
