using MediatR;

namespace Application.Commands.Tasks;

public record UpdateTaskItemCommand(
    Guid Id,
    string Title,
    string? Description,
    int StateId,
    Guid? AssigneeId,
    DateTime? DueDate
) : IRequest;
