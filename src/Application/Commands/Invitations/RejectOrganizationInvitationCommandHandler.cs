using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Invitations;

public class RejectOrganizationInvitationCommandHandler : BaseCommandHandler, IRequestHandler<RejectOrganizationInvitationCommand>
{
    public RejectOrganizationInvitationCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(RejectOrganizationInvitationCommand request, CancellationToken cancellationToken)
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
                throw new ValidationException("You are not authorized to reject this invitation");
            }

            if (invitation.Status != InvitationStatus.Pending)
            {
                throw new ValidationException($"Invitation has already been {invitation.Status.ToString().ToLower()}");
            }

            invitation.Status = InvitationStatus.Rejected;
            invitation.RespondedAt = DateTime.UtcNow;
            invitation.UpdatedAt = DateTime.UtcNow;
            unitOfWork.OrganizationInvitations.Update(invitation);
        }, cancellationToken);
    }
}
