using MediatR;

namespace Application.Commands.Boards;

public record DeleteBoardCommand(Guid Id) : IRequest;
