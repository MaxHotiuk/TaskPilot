using Application.Commands.Chats;
using Domain.Common.Authorization;
using Domain.Dtos.Chats;
using MediatR;

namespace WebApi.Endpoints.Chats;

public class UpdateChatReadStatusEndpoint : EndpointBaseWithRequest<UpdateChatReadStatusCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/chats/{chatId:guid}/read", async (
                Guid chatId,
                UpdateChatReadStatusRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateChatReadStatusCommand(chatId, dto.UserId, dto.ReadAt);
                await HandleAsync(command, mediator, cancellationToken);
                return Results.NoContent();
            })
            .WithName("UpdateChatReadStatus")
            .WithTags("Chats")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateChatReadStatusCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
