using Application.Commands.States;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Domain.Common.Authorization;

namespace WebApi.Endpoints.States;

public class CreateStateEndpoint : EndpointBaseWithRequest<CreateStateCommand, int>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/boards/{boardId:guid}/states", async (
                Guid boardId,
                CreateStateRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateStateCommand(
                    boardId,
                    dto.Name,
                    dto.Order
                );
                
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("CreateState")
            .WithTags("States")
            .RequireAuthorization(Policies.RequireBoardMemberOrOwner)
            .Produces<int>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(CreateStateCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Created($"/api/states/{result}", result);
    }
}

public record CreateStateRequestDto(string Name, int Order);
