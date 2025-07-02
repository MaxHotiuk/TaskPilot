using MediatR;

namespace Application.Commands.States;

public record DeleteStateCommand(int Id) : IRequest;
