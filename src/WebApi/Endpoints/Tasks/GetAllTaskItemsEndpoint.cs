using Application.Queries.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Dtos.Tasks;
using System.Collections.Generic;

namespace WebApi.Endpoints.Tasks;

public class GetAllTaskItemsEndpoint : EndpointBaseWithRequest<GetAllTaskItemsQuery, IEnumerable<TaskItemDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetAllTaskItemsQuery(), mediator, cancellationToken);
            })
            .WithName("GetAllTaskItems")
            .WithTags("Tasks")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetAllTaskItemsQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
