using Application.Queries.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Tasks;
using System.Collections.Generic;

namespace WebApi.Endpoints.Tasks;

public class GetTaskItemsByBoardIdEndpoint : EndpointBaseWithRequest<GetTaskItemsByBoardIdQuery, IEnumerable<TaskItemDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/{boardId:guid}/tasks", async (
                Guid boardId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetTaskItemsByBoardIdQuery(boardId), mediator, cancellationToken);
            })
            .WithName("GetTaskItemsByBoardId")
            .WithTags("Tasks")
            .RequireAuthorization() // Require authentication
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetTaskItemsByBoardIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
