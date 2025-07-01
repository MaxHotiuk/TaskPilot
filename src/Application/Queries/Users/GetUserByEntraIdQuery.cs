using Domain.Entities;
using MediatR;

namespace Application.Queries.Users;

public record GetUserByEntraIdQuery(string EntraId) : IRequest<User?>;
