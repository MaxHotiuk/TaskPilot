using MediatR;

namespace Application.Commands.States;

public record CreateStateCommand(
    Guid BoardId,
    string Name,
    int Order
) : IRequest<int>;
