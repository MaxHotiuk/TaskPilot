using Application.Queries.Tags;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;
using Domain.Dtos.Tags;

namespace WebApi.Endpoints.Tags;

public class GetTagsByBoardIdEndpoint : EndpointBaseWithRequest<GetTagsByBoardIdQuery, IEnumerable<TagDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/{boardId:guid}/tags", async (
                Guid boardId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetTagsByBoardIdQuery(boardId);
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("GetTagsByBoardId")
            .WithTags("Tags")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces<IEnumerable<TagDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetTagsByBoardIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
