using MediatR;

namespace Application.Commands.Tasks;

public record ArchiveTaskItemCommand(
    Guid TaskId
) : IRequest;