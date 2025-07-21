using Domain.Dtos.Users;
using MediatR;

namespace Application.Queries.Users;

public record GetCurrentUserQuery : IRequest<UserDto?>;
