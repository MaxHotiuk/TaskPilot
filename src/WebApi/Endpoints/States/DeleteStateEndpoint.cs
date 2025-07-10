using Application.Commands.States;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.States;

public class DeleteStateEndpoint : EndpointBaseWithRequest<DeleteStateCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/states/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteStateCommand(id);
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("DeleteState")
            .WithTags("States")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(DeleteStateCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
