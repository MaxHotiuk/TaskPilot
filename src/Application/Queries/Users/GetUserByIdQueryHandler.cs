using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Domain.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetUserByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IAuthenticationService _authenticationService;

    public GetUserByIdQueryHandler(IAuthenticationService authenticationService, IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
        _authenticationService = authenticationService;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                return null;
            }

            var entraId = await _authenticationService.GetCurrentUserEntraIdAsync();
            if (string.IsNullOrEmpty(entraId))
            {
                return null;
            }

            var currentUser = await unitOfWork.Users.GetByEntraIdAsync(entraId, cancellationToken);
            if (currentUser == null)
            {
                return null;
            }

            if (currentUser.Id == user.Id)
            {
                return user.ToDto();
            }

            var currentUserOrgIds = await unitOfWork.OrganizationMembers
                .GetOrganizationIdsByUserIdAsync(currentUser.Id, cancellationToken);
            var targetUserOrgIds = await unitOfWork.OrganizationMembers
                .GetOrganizationIdsByUserIdAsync(user.Id, cancellationToken);

            var sharedOrganizations = currentUserOrgIds.Intersect(targetUserOrgIds);
            if (!sharedOrganizations.Any())
            {
                return null;
            }

            return user.ToDto();
        }, cancellationToken);
    }
}
