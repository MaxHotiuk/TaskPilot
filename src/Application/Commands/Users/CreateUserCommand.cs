using MediatR;

namespace Application.Commands.Users;

public record CreateUserCommand(
    string Email,
    string Username,
    string EntraId,
    string Role
) : IRequest<Guid>;
