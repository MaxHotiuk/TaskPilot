using MediatR;

namespace Application.Commands.Boards;

public record UpdateBoardCommand(
    Guid Id,
    string Name,
    string? Description
) : IRequest;
