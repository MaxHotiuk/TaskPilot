using Domain.Entities;
using MediatR;

namespace Application.Queries.Users;

public record GetAllUsersQuery : IRequest<IEnumerable<User>>;
