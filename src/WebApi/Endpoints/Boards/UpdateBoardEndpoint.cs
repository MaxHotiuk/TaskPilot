using Application.Commands.Boards;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Boards;

public class UpdateBoardEndpoint : EndpointBaseWithRequest<UpdateBoardCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/boards/{id:guid}", async (
                Guid id,
                UpdateBoardRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateBoardCommand(
                    id,
                    dto.Name,
                    dto.Description
                );
                
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateBoard")
            .WithTags("Boards")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateBoardCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record UpdateBoardRequestDto(string Name, string? Description);
