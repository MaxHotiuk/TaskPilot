using MediatR;

namespace Application.Commands.Boards;

public record CreateBoardCommand(
    string Name,
    string? Description,
    Guid OwnerId
) : IRequest<Guid>;
