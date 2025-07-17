using Application.Queries.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Boards;
using System.Collections.Generic;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Boards;

public class GetArchivedBoardsByOwnerEndpoint : EndpointBaseWithRequest<GetArchivedBoardsByOwnerQuery, IEnumerable<BoardDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{ownerId:guid}/boards/archived", async (
                Guid ownerId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetArchivedBoardsByOwnerQuery(ownerId), mediator, cancellationToken);
            })
            .WithName("GetArchivedBoardsByOwner")
            .WithTags("Boards")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetArchivedBoardsByOwnerQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
