using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Users;

public class CreateUserCommandHandler : BaseCommandHandler, IRequestHandler<CreateUserCommand, Guid>
{
    public CreateUserCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            if (await unitOfWork.Users.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                throw new ValidationException("User with this email already exists");
            }

            if (await unitOfWork.Users.ExistsByEntraIdAsync(request.EntraId, cancellationToken))
            {
                throw new ValidationException("User with this EntraId already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Username = request.Username,
                EntraId = request.EntraId,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Users.AddAsync(user, cancellationToken);

            await EnsureUserInOrganizationAsync(user, unitOfWork, cancellationToken);

            return user.Id;
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
