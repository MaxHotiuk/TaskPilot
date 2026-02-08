using Application.Commands.Chats;
using Domain.Common.Authorization;
using Domain.Dtos.Chats;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class UpdateChatNameEndpoint : EndpointBaseWithRequest<UpdateChatNameCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/chats/{chatId:guid}/name", async (
                Guid chatId,
                UpdateChatNameRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateChatNameCommand(chatId, dto.UserId, dto.Name);
                await HandleAsync(command, mediator, cancellationToken);
                return Results.NoContent();
            })
            .WithName("UpdateChatName")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateChatNameCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
