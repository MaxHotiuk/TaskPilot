using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Dtos.Users;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IAuthenticationService authenticationService, IUserRepository userRepository)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
    }

    public async Task<UserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var entraId = await _authenticationService.GetCurrentUserEntraIdAsync();
        
        if (string.IsNullOrEmpty(entraId))
        {
            return null;
        }

        var user = await _userRepository.GetByEntraIdAsync(entraId, cancellationToken);
        return user?.ToDto();
    }
}
