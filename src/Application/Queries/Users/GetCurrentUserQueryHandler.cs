using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetCurrentUserQueryHandler : BaseQueryHandler, IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    private readonly IAuthenticationService _authenticationService;

    public GetCurrentUserQueryHandler(IAuthenticationService authenticationService, IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
        _authenticationService = authenticationService;
    }

    public async Task<UserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var entraId = await _authenticationService.GetCurrentUserEntraIdAsync();
        
        if (string.IsNullOrEmpty(entraId))
        {
            return null;
        }

        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByEntraIdAsync(entraId, cancellationToken);
            return user?.ToDto();
        }, cancellationToken);
    }
}
