using Application.Commands.Chats;
using Domain.Common.Authorization;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class ClearChatHistoryEndpoint : EndpointBaseWithRequest<ClearChatHistoryCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/chats/{chatId:guid}/messages", async (
                Guid chatId,
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new ClearChatHistoryCommand(chatId, userId);
                await HandleAsync(command, mediator, cancellationToken);
                return Results.NoContent();
            })
            .WithName("ClearChatHistory")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(ClearChatHistoryCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
