using Application.Commands.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Boards;

public class CreateBoardEndpoint : EndpointBaseWithRequest<CreateBoardCommand, Guid>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/boards", async (
                CreateBoardRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateBoardCommand(
                    dto.Name,
                    dto.Description,
                    dto.OwnerId
                );
                
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("CreateBoard")
            .WithTags("Boards")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(CreateBoardCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Created($"/api/boards/{result}", result);
    }
}

public record CreateBoardRequestDto(string Name, string? Description, Guid OwnerId);
