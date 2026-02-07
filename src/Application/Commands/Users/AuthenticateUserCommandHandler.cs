using Application.Abstractions.Persistence;
using Domain.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Users;

public class AuthenticateUserCommandHandler : BaseCommandHandler, IRequestHandler<AuthenticateUserCommand, UserDto>
{
    public AuthenticateUserCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<UserDto> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var existingUser = await unitOfWork.Users.GetByEntraIdAsync(request.EntraId, cancellationToken);

            if (existingUser != null)
            {
                if (existingUser.Email != request.Email || existingUser.Username != request.Username)
                {
                    existingUser.Email = request.Email;
                    existingUser.Username = request.Username;
                    existingUser.UpdatedAt = DateTime.UtcNow;

                    unitOfWork.Users.Update(existingUser);
                }

                await EnsureUserInOrganizationAsync(existingUser, unitOfWork, cancellationToken);

                return existingUser.ToDto();
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                EntraId = request.EntraId,
                Email = request.Email,
                Username = request.Username,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Users.AddAsync(newUser, cancellationToken);

            await EnsureUserInOrganizationAsync(newUser, unitOfWork, cancellationToken);

            return newUser.ToDto();
        }, cancellationToken);
    }

    private async Task EnsureUserInOrganizationAsync(User user, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var domain = user.Email.Split('@').LastOrDefault();
        if (string.IsNullOrEmpty(domain))
        {
            return;
        }

        var organization = await unitOfWork.Organizations.GetByDomainAsync(domain, cancellationToken);
        if (organization == null)
        {
            organization = new Organization
            {
                Id = Guid.NewGuid(),
                Domain = domain,
                Name = domain,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await unitOfWork.Organizations.AddAsync(organization, cancellationToken);
        }

        var isMember = await unitOfWork.OrganizationMembers.IsMemberOfOrganizationAsync(
            organization.Id, user.Id, cancellationToken);

        if (!isMember)
        {
            var organizationMember = new OrganizationMember
            {
                OrganizationId = organization.Id,
                UserId = user.Id,
                IsInvited = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await unitOfWork.OrganizationMembers.AddAsync(organizationMember, cancellationToken);
        }
    }
}
