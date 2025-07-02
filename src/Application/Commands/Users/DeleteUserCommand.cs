using MediatR;

namespace Application.Commands.Users;

public record DeleteUserCommand(Guid Id) : IRequest;
