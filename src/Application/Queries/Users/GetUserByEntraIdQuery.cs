using Application.Common.Dtos.Users;
using MediatR;

namespace Application.Queries.Users;

public record GetUserByEntraIdQuery(string EntraId) : IRequest<UserDto?>;
