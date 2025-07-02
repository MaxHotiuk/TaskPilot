using Application.Abstractions.Persistence;
using Application.Common.Dtos.Users;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        return user?.ToDto();
    }
}
