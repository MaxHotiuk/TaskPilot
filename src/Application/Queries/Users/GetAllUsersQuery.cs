using Application.Common.Dtos.Users;
using MediatR;

namespace Application.Queries.Users;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
