using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Invitations;

public class AcceptOrganizationInvitationCommandHandler : BaseCommandHandler, IRequestHandler<AcceptOrganizationInvitationCommand>
{
    public AcceptOrganizationInvitationCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(AcceptOrganizationInvitationCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var invitation = await unitOfWork.OrganizationInvitations.GetByIdAsync(request.InvitationId, cancellationToken);
            if (invitation is null)
            {
                throw new NotFoundException($"Invitation with ID {request.InvitationId} not found");
            }

            // Verify that the current user is the invited user
            if (invitation.UserId != request.CurrentUserId)
            {
                throw new ValidationException("You are not authorized to accept this invitation");
            }

            if (invitation.Status != InvitationStatus.Pending)
            {
                throw new ValidationException($"Invitation has already been {invitation.Status.ToString().ToLower()}");
            }

            var organization = await unitOfWork.Organizations.GetByIdAsync(invitation.OrganizationId, cancellationToken);
            if (organization is null)
            {
                throw new ValidationException($"Organization with ID {invitation.OrganizationId} does not exist");
            }

            // Check if user is already a member
            var existingMember = await unitOfWork.OrganizationMembers.GetOrganizationMemberAsync(
                invitation.OrganizationId, 
                invitation.UserId, 
                cancellationToken);

            if (existingMember != null)
            {
                throw new ValidationException("User is already a member of this organization");
            }

            // Update invitation status
            invitation.Status = InvitationStatus.Accepted;
            invitation.RespondedAt = DateTime.UtcNow;
            invitation.UpdatedAt = DateTime.UtcNow;
            unitOfWork.OrganizationInvitations.Update(invitation);

            // Add user to organization
            var organizationMember = new OrganizationMember
            {
                OrganizationId = invitation.OrganizationId,
                UserId = invitation.UserId,
                Role = invitation.Role,
                IsInvited = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.OrganizationMembers.AddAsync(organizationMember, cancellationToken);
        }, cancellationToken);
    }
}
