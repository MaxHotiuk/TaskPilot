using MediatR;

namespace Application.Commands.Tasks;

public record DeleteTaskItemCommand(Guid Id) : IRequest;
