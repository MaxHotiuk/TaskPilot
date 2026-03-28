using Application.Commands.Chats;
using Domain.Common.Authorization;
using Domain.Dtos.Chats;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class SendChatMessageEndpoint : EndpointBaseWithRequest<SendChatMessageCommand, ChatMessageDto>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chats/{chatId:guid}/messages", async (
                Guid chatId,
                SendChatMessageRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new SendChatMessageCommand(
                    chatId,
                    dto.SenderId,
                    dto.Content
                );

                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("SendChatMessage")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<ChatMessageDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SendChatMessageCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
