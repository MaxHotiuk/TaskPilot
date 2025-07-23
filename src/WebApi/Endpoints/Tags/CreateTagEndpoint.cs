using Application.Commands.Tags;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;
using Domain.Dtos.Tags;

namespace WebApi.Endpoints.Tags;

public class CreateTagEndpoint : EndpointBaseWithRequest<CreateTagCommand, int>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/boards/{boardId:guid}/tags", async (
                Guid boardId,
                CreateTagRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateTagCommand
                {
                    BoardId = boardId,
                    Name = dto.Name,
                    Color = dto.Color
                };
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("CreateTag")
            .WithTags("Tags")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces<int>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(CreateTagCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Created($"/api/tags/{result}", result);
    }
}
