using MediatR;

namespace Application.Commands.Tasks;

public record CreateTaskItemCommand(
    Guid BoardId,
    string Title,
    string? Description,
    int StateId,
    int? TagId,
    int Priority,
    Guid? AssigneeId,
    DateTime? DueDate
) : IRequest<Guid>;
