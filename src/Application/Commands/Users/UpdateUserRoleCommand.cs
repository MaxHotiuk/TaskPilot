using MediatR;

namespace Application.Commands.Users;

public record UpdateUserRoleCommand(
    Guid UserId,
    string Role
) : IRequest;
