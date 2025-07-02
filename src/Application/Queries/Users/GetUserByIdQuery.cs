using Application.Common.Dtos.Users;
using MediatR;

namespace Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
