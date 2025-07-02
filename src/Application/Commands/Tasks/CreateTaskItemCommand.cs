using MediatR;

namespace Application.Commands.Tasks;

public record CreateTaskItemCommand(
    Guid BoardId,
    string Title,
    string? Description,
    int StateId,
    Guid? AssigneeId,
    DateTime? DueDate
) : IRequest<Guid>;
