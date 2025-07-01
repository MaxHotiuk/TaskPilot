using Application.Abstractions.Persistence;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Users;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, User?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
    }
}
