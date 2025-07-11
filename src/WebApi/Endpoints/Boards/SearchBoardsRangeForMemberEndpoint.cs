using Application.Queries.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Boards;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Boards;

public class SearchBoardsRangeForMemberEndpoint : EndpointBaseWithRequest<SearchBoardsRangeForMemberQuery, IEnumerable<BoardSearchDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/member/search", async (
                Guid userId,
                string searchTerm,
                int page,
                int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new SearchBoardsRangeForMemberQuery(userId, searchTerm, page, pageSize);
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("SearchBoardsRangeForMember")
            .WithTags("Boards")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<BoardSearchDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SearchBoardsRangeForMemberQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
