using MediatR;

namespace Application.Commands.Tasks;

public record UpdateTaskItemCommand(
    Guid Id,
    string Title,
    string? Description,
    int StateId,
    Guid? AssigneeId,
    int? TagId,
    int Priority,
    DateTime? DueDate
) : IRequest;
