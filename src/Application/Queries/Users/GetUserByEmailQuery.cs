using Domain.Entities;
using MediatR;

namespace Application.Queries.Users;

public record GetUserByEmailQuery(string Email) : IRequest<User?>;
