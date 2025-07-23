using Application.Commands.Tags;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;
using Domain.Dtos.Tags;

namespace WebApi.Endpoints.Tags;

public class UpdateTagEndpoint : EndpointBaseWithRequest<UpdateTagCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/boards/{boardId:guid}/tags/{id:int}", async (
                Guid boardId,
                int id,
                UpdateTagRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateTagCommand
                {
                    Id = id,
                    BoardId = boardId,
                    Name = dto.Name,
                    Color = dto.Color
                };
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateTag")
            .WithTags("Tags")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateTagCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}
