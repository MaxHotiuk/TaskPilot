using Application.Queries.Comments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Application.Common.Dtos.Comments;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.Comments;

public class GetCommentByIdEndpoint : EndpointBaseWithRequest<GetCommentByIdQuery, CommentDto?>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/comments/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return await HandleAsync(new GetCommentByIdQuery(id), mediator, cancellationToken);
            })
            .WithName("GetCommentById")
            .WithTags("Comments")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<CommentDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(GetCommentByIdQuery request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return HandleResult(result);
    }
}
