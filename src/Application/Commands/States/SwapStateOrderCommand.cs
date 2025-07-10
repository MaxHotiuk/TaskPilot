using MediatR;

namespace Application.Commands.States;

public record SwapStateOrderCommand(
    int FirstStateId,
    int SecondStateId,
    Guid BoardId
) : IRequest;
