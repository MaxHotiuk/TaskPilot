using Application.Abstractions.Messaging;

namespace Application.Commands.Users;

public record DeleteUserCommand(Guid Id) : ICommand;
