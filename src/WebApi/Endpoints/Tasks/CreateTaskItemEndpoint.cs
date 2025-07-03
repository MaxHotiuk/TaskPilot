using Application.Commands.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WebApi.Endpoints.Tasks;

public class CreateTaskItemEndpoint : EndpointBaseWithRequest<CreateTaskItemCommand, Guid>
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks", async (
                CreateTaskItemRequestDto dto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateTaskItemCommand(
                    dto.BoardId,
                    dto.Title,
                    dto.Description,
                    dto.StateId,
                    dto.AssigneeId,
                    dto.DueDate
                );
                
                return await HandleAsync(command, mediator, cancellationToken);
            })
            .WithName("CreateTaskItem")
            .WithTags("Tasks")
            .RequireAuthorization() // Require authentication
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    public override async Task<IResult> HandleAsync(CreateTaskItemCommand request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Created($"/api/tasks/{result}", result);
    }
}

public record CreateTaskItemRequestDto(
    Guid BoardId,
    string Title,
    string? Description,
    int StateId,
    Guid? AssigneeId,
    DateTime? DueDate);
