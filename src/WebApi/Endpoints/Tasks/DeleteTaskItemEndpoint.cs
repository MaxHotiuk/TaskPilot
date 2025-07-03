using Application.Commands.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Tasks;

public class DeleteTaskItemEndpoint : EndpointBaseWithRequest<DeleteTaskItemCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/tasks/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new DeleteTaskItemCommand(id), mediator, cancellationToken);
            })
            .WithName("DeleteTaskItem")
            .WithTags("Tasks")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(DeleteTaskItemCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
