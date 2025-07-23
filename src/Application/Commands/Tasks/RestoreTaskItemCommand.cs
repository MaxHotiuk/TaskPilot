using MediatR;

namespace Application.Commands.Tasks;

public record RestoreTaskItemCommand(
    Guid TaskId
) : IRequest;