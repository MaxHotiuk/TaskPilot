using Domain.Entities;
using MediatR;

namespace Application.Queries.Users;

public record GetUserByIdQuery(Guid Id) : IRequest<User?>;
