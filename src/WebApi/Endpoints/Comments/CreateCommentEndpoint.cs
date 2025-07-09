using Application.Commands.Comments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;
using WebApi.Dtos.Comments;

namespace WebApi.Endpoints.Comments;

public class CreateCommentEndpoint : EndpointBaseWithRequest<CreateCommentCommand, Guid>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/comments", async (
                CreateCommentRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateCommentCommand(
                    dto.TaskId,
                    dto.AuthorId,
                    dto.Content
                );
                
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("CreateComment")
            .WithTags("Comments")
            .RequireAuthorization(Policies.RequireUserRole)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(CreateCommentCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Created($"/api/comments/{result}", result);
    }
}
