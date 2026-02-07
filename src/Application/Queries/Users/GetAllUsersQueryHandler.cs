using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Domain.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetAllUsersQueryHandler : BaseQueryHandler, IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IAuthenticationService _authenticationService;

    public GetAllUsersQueryHandler(IAuthenticationService authenticationService, IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
        _authenticationService = authenticationService;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var entraId = await _authenticationService.GetCurrentUserEntraIdAsync();
            if (string.IsNullOrEmpty(entraId))
            {
                return Enumerable.Empty<UserDto>();
            }

            var currentUser = await unitOfWork.Users.GetByEntraIdAsync(entraId, cancellationToken);
            if (currentUser == null)
            {
                return Enumerable.Empty<UserDto>();
            }

            var organizationIds = await unitOfWork.OrganizationMembers
                .GetOrganizationIdsByUserIdAsync(currentUser.Id, cancellationToken);

            if (!organizationIds.Any())
            {
                return Enumerable.Empty<UserDto>();
            }

            var users = await unitOfWork.Users.GetByOrganizationIdsAsync(organizationIds, cancellationToken);

            return users.ToDto();
        }, cancellationToken);
    }
}
