using Application.Abstractions.Persistence;
using Application.Common.Dtos.Users;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetUserByEntraIdQueryHandler : IRequestHandler<GetUserByEntraIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEntraIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetUserByEntraIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEntraIdAsync(request.EntraId, cancellationToken);
        return user?.ToDto();
    }
}
