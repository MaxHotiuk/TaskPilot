using MediatR;

namespace Application.Commands.Users;

public record UpdateUserCommand(
    Guid Id,
    string Email,
    string Username,
    string Role
) : IRequest;
