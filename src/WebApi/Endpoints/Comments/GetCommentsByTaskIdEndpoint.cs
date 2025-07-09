using Application.Queries.Comments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Comments;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Comments;

public class GetCommentsByTaskIdEndpoint : EndpointBaseWithRequest<GetCommentsByTaskIdQuery, IEnumerable<CommentDto>>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/{taskId:guid}/comments", async (
                Guid taskId,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetCommentsByTaskIdQuery(taskId), mediator, cancellationToken);
            })
            .WithName("GetCommentsByTaskId")
            .WithTags("Comments")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<IEnumerable<CommentDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetCommentsByTaskIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
