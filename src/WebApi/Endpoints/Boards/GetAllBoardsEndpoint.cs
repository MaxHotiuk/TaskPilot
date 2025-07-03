using Application.Queries.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Boards;
using System.Collections.Generic;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Boards;

public class GetAllBoardsEndpoint : EndpointBaseWithRequest<GetAllBoardsQuery, IEnumerable<BoardDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards", async (
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetAllBoardsQuery(), mediator, cancellationToken);
            })
            .WithName("GetAllBoards")
            .WithTags("Boards")
            .RequireAuthorization(Policies.RequireAdminRole)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetAllBoardsQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
