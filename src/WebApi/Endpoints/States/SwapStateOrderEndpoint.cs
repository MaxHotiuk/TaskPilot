using Application.Commands.States;
using MediatR;
using Domain.Common.Authorization;
using Application.Common.Dtos.States;

namespace WebApi.Endpoints.States;

public class SwapStateOrderEndpoint : EndpointBaseWithRequest<SwapStateOrderCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/boards/{boardId:guid}/states/swap-order", async (
                Guid boardId,
                SwapStateOrderRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new SwapStateOrderCommand(
                    dto.FirstStateId,
                    dto.SecondStateId,
                    boardId
                );
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("SwapStateOrder")
            .WithTags("States")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SwapStateOrderCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
