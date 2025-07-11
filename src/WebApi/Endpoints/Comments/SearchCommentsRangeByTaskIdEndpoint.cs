using Application.Queries.Comments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Comments;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Comments;

public class SearchCommentsRangeByTaskIdEndpoint : EndpointBaseWithRequest<SearchCommentsRangeByTaskIdQuery, IEnumerable<CommentDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/comments/search", async (
                string searchTerm,
                Guid taskId,
                int page,
                int pageSize,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new SearchCommentsRangeByTaskIdQuery(searchTerm, taskId, page, pageSize);
                return await HandleAsync(query, mediator, cancellationToken);
            })
            .WithName("SearchCommentsRangeByTaskId")
            .WithTags("Comments")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<CommentDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(SearchCommentsRangeByTaskIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
