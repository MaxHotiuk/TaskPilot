using Application.Commands.Comments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;
using Domain.Dtos.Comments;

namespace WebApi.Endpoints.Comments;

public class UpdateCommentEndpoint : EndpointBaseWithRequest<UpdateCommentCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/comments/{id:guid}", async (
                Guid id,
                UpdateCommentRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateCommentCommand(
                    id,
                    dto.Content
                );
                
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateComment")
            .WithTags("Comments")
            .RequireAuthorization(Policies.RequireCommentOwner)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateCommentCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
