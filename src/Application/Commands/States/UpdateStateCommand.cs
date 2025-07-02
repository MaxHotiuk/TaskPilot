using MediatR;

namespace Application.Commands.States;

public record UpdateStateCommand(
    int Id,
    string Name,
    int Order
) : IRequest;
