using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
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

            // Verify the organization exists
            var organization = await unitOfWork.Organizations.GetByIdAsync(request.OrganizationId, cancellationToken);
            if (organization == null)
            {
                throw new NotFoundException($"Organization with ID {request.OrganizationId} not found");
            }

            // Verify current user is a member of the requested organization
            var isMember = await unitOfWork.OrganizationMembers.IsMemberOfOrganizationAsync(
                request.OrganizationId, 
                currentUser.Id, 
                cancellationToken);

            if (!isMember)
            {
                throw new ValidationException("You must be a member of this organization to view its users");
            }

            // Get all users in the organization
            var users = await unitOfWork.Users.GetByOrganizationIdsAsync(
                new[] { request.OrganizationId }, 
                cancellationToken);

            return users.ToDto();
        }, cancellationToken);
    }
}
