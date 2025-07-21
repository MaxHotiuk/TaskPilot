using Application.Queries.BoardMembers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Dtos.BoardMembers;
using System.Collections.Generic;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.BoardMembers;

public class GetBoardMembersByBoardIdEndpoint : EndpointBaseWithRequest<GetBoardMembersByBoardIdQuery, IEnumerable<BoardMemberDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/{boardId:guid}/members", async (
                Guid boardId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetBoardMembersByBoardIdQuery(boardId), mediator, cancellationToken);
            })
            .WithName("GetBoardMembersByBoardId")
            .WithTags("BoardMembers")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetBoardMembersByBoardIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
