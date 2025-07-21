using Domain.Dtos.Users;
using MediatR;

namespace Application.Commands.Users;

public record AuthenticateUserCommand(
    string EntraId,
    string Email,
    string Username
) : IRequest<UserDto>;
