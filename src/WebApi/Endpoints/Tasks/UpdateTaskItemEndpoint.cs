using Application.Commands.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Tasks;

public class UpdateTaskItemEndpoint : EndpointBaseWithRequest<UpdateTaskItemCommand>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/tasks/{id:guid}", async (
                Guid id,
                UpdateTaskItemRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateTaskItemCommand(
                    id,
                    dto.Title,
                    dto.Description,
                    dto.StateId,
                    dto.AssigneeId,
                    dto.DueDate
                );
                
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("UpdateTaskItem")
            .WithTags("Tasks")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(UpdateTaskItemCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken);
        return Results.NoContent();
    }
}

public record UpdateTaskItemRequestDto(
    string Title,
    string? Description,
    int StateId,
    Guid? AssigneeId,
    DateTime? DueDate);
