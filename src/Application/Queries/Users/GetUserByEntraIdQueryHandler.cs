using Application.Abstractions.Persistence;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Users;

public class GetUserByEntraIdQueryHandler : IRequestHandler<GetUserByEntraIdQuery, User?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEntraIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(GetUserByEntraIdQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByEntraIdAsync(request.EntraId, cancellationToken);
    }
}
