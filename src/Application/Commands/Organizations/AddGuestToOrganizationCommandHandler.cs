using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Organizations;

public record AddGuestToOrganizationCommand(Guid OrganizationId, string UserEmail, Guid ManagerId) : ICommand<Unit>;

public class AddGuestToOrganizationCommandHandler : BaseCommandHandler, IRequestHandler<AddGuestToOrganizationCommand, Unit>
{
    private readonly IEmailService _emailService;
    private readonly IOrganizationMemberRepository _organizationMemberRepository;

    public AddGuestToOrganizationCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IEmailService emailService,
        IOrganizationMemberRepository organizationMemberRepository) 
        : base(unitOfWorkFactory)
    {
        _emailService = emailService;
        _organizationMemberRepository = organizationMemberRepository;
    }

    public async Task<Unit> Handle(AddGuestToOrganizationCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            // Verify manager is a manager of this organization
            var isManager = await _organizationMemberRepository.IsManagerOfOrganizationAsync(
                request.OrganizationId, 
                request.ManagerId, 
                cancellationToken);

            if (!isManager)
                throw new ValidationException("Only organization managers can add guests to their organization");

            // Verify organization exists
            var organization = await unitOfWork.Organizations.GetByIdAsync(request.OrganizationId, cancellationToken);
            if (organization == null)
                throw new NotFoundException("Organization not found");

            // Verify user exists
            var user = await unitOfWork.Users.GetByEmailAsync(request.UserEmail, cancellationToken);
            if (user == null)
                throw new NotFoundException("User not found");

            // Check if user is already a member of this organization
            var existingMembership = await _organizationMemberRepository.GetOrganizationMemberAsync(
                request.OrganizationId, 
                user.Id, 
                cancellationToken);

            if (existingMembership != null)
                throw new ValidationException("User is already a member of this organization");

            // Check if there's already a pending invitation
            if (await unitOfWork.OrganizationInvitations.HasPendingInvitationAsync(request.OrganizationId, user.Id, cancellationToken))
            {
                throw new ValidationException("User already has a pending invitation for this organization");
            }

            // Verify user belongs to at least one other organization (as a full member)
            var userOrganizations = await _organizationMemberRepository.GetByUserIdAsync(user.Id, cancellationToken);
            var hasPrimaryOrganization = userOrganizations.Any(om => om.Role != OrganizationMemberRole.Guest);

            if (!hasPrimaryOrganization)
                throw new ValidationException("User must belong to at least one organization as a full member before being added as a guest to another organization");

            var manager = await unitOfWork.Users.GetByIdAsync(request.ManagerId, cancellationToken);
            if (manager == null)
                throw new NotFoundException("Manager not found");

            // Create invitation
            var invitation = new OrganizationInvitation
            {
                Id = Guid.NewGuid(),
                OrganizationId = request.OrganizationId,
                UserId = user.Id,
                InvitedBy = request.ManagerId,
                Role = OrganizationMemberRole.Guest,
                Status = InvitationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.OrganizationInvitations.AddAsync(invitation, cancellationToken);

            // Send invitation email
            var emailBody = $@"
                <p>Hello {user.Username ?? user.Email},</p>
                <p><strong>{manager.Username ?? manager.Email}</strong> has invited you to join <strong>{organization.Name}</strong> organization as a guest.</p>
                <p>As a guest, you can:</p>
                <ul>
                    <li>Participate in chats and calls</li>
                    <li>Be added to boards and work on tasks</li>
                    <li>Collaborate with team members</li>
                </ul>
                <p>Please log in to TaskPilot to accept or decline this invitation.</p>
                <p>Best regards,<br/>TaskPilot Team</p>";

            await _emailService.SendSystemEmailAsync(
                user.Email,
                $"Guest Invitation to {organization.Name}",
                emailBody,
                cancellationToken);

            return Unit.Value;
        }, cancellationToken);
    }
}
