using Application.Queries.States;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.States;
using System.Collections.Generic;

namespace WebApi.Endpoints.States;

public class GetStatesByBoardIdEndpoint : EndpointBaseWithRequest<GetStatesByBoardIdQuery, IEnumerable<StateDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/{boardId:guid}/states", async (
                Guid boardId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetStatesByBoardIdQuery(boardId), mediator, cancellationToken);
            })
            .WithName("GetStatesByBoardId")
            .WithTags("States")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetStatesByBoardIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
