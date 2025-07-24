using MediatR;
using Domain.Common.Authorization;
using Application.Queries.Backlog;
using Domain.Dtos.Backlog;

namespace WebApi.Endpoints.Backlog;

public class SearchBacklogRangeByBoardIdEndpoint : EndpointBaseWithRequest<SearchBacklogRangeByBoardIdQuery, IEnumerable<BacklogDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/{boardId}/backlog/search", async (
                Guid boardId,
                string searchTerm,
                int page,
                int pageSize,
                DateOnly startDate,
                DateOnly endDate,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new SearchBacklogRangeByBoardIdQuery(
                    boardId,
                    searchTerm,
                    startDate,
                    endDate,
                    page,
                    pageSize);
                Console.WriteLine($"Searching backlog for board {boardId} with search term '{searchTerm}' from {startDate} to {endDate}, page {page}, page size {pageSize}");
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("SearchBacklogRangeByBoardId")
            .WithTags("Backlog")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<BacklogDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SearchBacklogRangeByBoardIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
