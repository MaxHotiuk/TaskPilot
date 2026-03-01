using Application.Queries.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Dtos.Boards;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Boards;

public class SearchBoardsRangeForOwnerEndpoint : EndpointBaseWithRequest<SearchBoardsRangeForOwnerQuery, IEnumerable<BoardSearchDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/boards/owner/search", async (
                Guid ownerId,
                Guid organizationId,
                string searchTerm,
                int page,
                int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new SearchBoardsRangeForOwnerQuery(ownerId, organizationId, searchTerm, page, pageSize);
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("SearchBoardsRangeForOwner")
            .WithTags("Boards")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<BoardSearchDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SearchBoardsRangeForOwnerQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
