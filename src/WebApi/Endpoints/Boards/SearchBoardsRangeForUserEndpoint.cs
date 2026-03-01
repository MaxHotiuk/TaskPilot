using Application.Queries.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Dtos.Boards;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Boards;

public class SearchBoardsRangeForUserEndpoint : EndpointBaseWithRequest<SearchBoardsRangeForUserQuery, IEnumerable<BoardSearchDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/user/search", async (
                Guid userId,
                Guid organizationId,
                string searchTerm,
                int page,
                int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new SearchBoardsRangeForUserQuery(userId, organizationId, searchTerm, page, pageSize);
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("SearchBoardsRangeForUser")
            .WithTags("Boards")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<BoardSearchDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SearchBoardsRangeForUserQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
