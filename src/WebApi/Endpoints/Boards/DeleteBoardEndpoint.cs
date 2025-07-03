using Application.Commands.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Boards;

public class DeleteBoardEndpoint : EndpointBaseWithRequest<DeleteBoardCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/boards/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new DeleteBoardCommand(id), mediator, cancellationToken);
            })
            .WithName("DeleteBoard")
            .WithTags("Boards")
            .RequireAuthorization(Policies.RequireBoardOwner)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(DeleteBoardCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
