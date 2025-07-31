using Application.Queries.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;
using Domain.Dtos.Tasks;

namespace WebApi.Endpoints.Tasks;

public class SearchArchivedRangeTaskItemsEndpoint : EndpointBaseWithRequest<SearchArchivedRangeTaskItemsQuery, IEnumerable<ArchivedTaskDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/archived", async (
                int page,
                int pageSize,
                string searchTerm,
                Guid boardId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new SearchArchivedRangeTaskItemsQuery(page, pageSize, boardId, searchTerm), mediator, cancellationToken);
            })
            .WithName("SearchArchivedRangeTaskItems")
            .WithTags("Tasks")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces<IEnumerable<TaskItemDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SearchArchivedRangeTaskItemsQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
