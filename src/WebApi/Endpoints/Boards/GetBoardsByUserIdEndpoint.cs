using Application.Queries.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Dtos.Boards;
using System.Collections.Generic;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Boards;

public class GetBoardsByUserIdEndpoint : EndpointBaseWithRequest<GetBoardsByUserIdQuery, IEnumerable<BoardDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{userId:guid}/boards", async (
                Guid userId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetBoardsByUserIdQuery(userId), mediator, cancellationToken);
            })
            .WithName("GetBoardsByUserId")
            .WithTags("Boards")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetBoardsByUserIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
