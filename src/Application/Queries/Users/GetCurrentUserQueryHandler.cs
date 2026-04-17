using Application.Abstractions.Authentication;
using Application.Abstractions.Persistence;
using Domain.Dtos.Organizations;
using Domain.Dtos.Users;
using Application.Common.Handlers;
using MediatR;

namespace Application.Queries.Users;

public class GetCurrentUserQueryHandler : BaseQueryHandler, IRequestHandler<GetCurrentUserQuery, CurrentUserDto?>
{
    private readonly IAuthenticationService _authenticationService;

    public GetCurrentUserQueryHandler(IAuthenticationService authenticationService, IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
        _authenticationService = authenticationService;
    }

    public async Task<CurrentUserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var entraId = await _authenticationService.GetCurrentUserEntraIdAsync();
        
        if (string.IsNullOrEmpty(entraId))
        {
            return null;
        }

        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByEntraIdAsync(entraId, cancellationToken);
            if (user is null)
            {
                return null;
            }

            var organizations = await unitOfWork.Organizations.GetOrganizationsByUserIdAsync(user.Id, cancellationToken);
            var organizationMembers = await unitOfWork.OrganizationMembers.GetByUserIdAsync(user.Id, cancellationToken);

            var organizationDtos = organizations.Select(org =>
            {
                var memberInfo = organizationMembers.FirstOrDefault(om => om.OrganizationId == org.Id);
                return new OrganizationSummaryDto
                {
                    Id = org.Id,
                    Name = org.Name,
                    Role = memberInfo?.Role.ToString() ?? "Member"
                };
            });

            return new CurrentUserDto
            {
                Id = user.Id,
                EntraId = user.EntraId,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsGoogleCalendarConnected = user.IsGoogleCalendarConnected,
                Organizations = organizationDtos.ToList()
            };
        }, cancellationToken);
    }
}
