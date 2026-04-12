using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.BoardMembers;

public class AddBoardMemberCommandHandler : BaseCommandHandler, IRequestHandler<AddBoardMemberCommand>
{
    private readonly IEmailService _emailService;

    public AddBoardMemberCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IEmailService emailService)
        : base(unitOfWorkFactory)
    {
        _emailService = emailService;
    }

    public async Task Handle(AddBoardMemberCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {request.BoardId} does not exist");
            }

            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new ValidationException($"User with ID {request.UserId} does not exist");
            }

            if (await unitOfWork.BoardMembers.IsMemberOfBoardAsync(request.BoardId, request.UserId, cancellationToken))
            {
                throw new ValidationException($"User {request.UserId} is already a member of board {request.BoardId}");
            }

            // Check if there's already a pending invitation
            if (await unitOfWork.BoardInvitations.HasPendingInvitationAsync(request.BoardId, request.UserId, cancellationToken))
            {
                throw new ValidationException($"User {request.UserId} already has a pending invitation for board {request.BoardId}");
            }

            var boardOwner = await unitOfWork.Users.GetByIdAsync(board.OwnerId, cancellationToken);
            if (boardOwner is null)
            {
                throw new ValidationException($"Board owner with ID {board.OwnerId} does not exist");
            }

            var ownerOrgIds = await unitOfWork.OrganizationMembers
                .GetOrganizationIdsByUserIdAsync(board.OwnerId, cancellationToken);
            var userOrgIds = await unitOfWork.OrganizationMembers
                .GetOrganizationIdsByUserIdAsync(request.UserId, cancellationToken);

            var sharedOrganizations = ownerOrgIds.Intersect(userOrgIds).ToList();
            if (!sharedOrganizations.Any())
            {
                throw new ValidationException($"User {request.UserId} is not in the same organization as the board owner");
            }

            if (!request.InvitedBy.HasValue)
            {
                throw new ValidationException("InvitedBy is required");
            }

            var inviter = await unitOfWork.Users.GetByIdAsync(request.InvitedBy.Value, cancellationToken);
            if (inviter is null)
            {
                throw new ValidationException($"Inviter with ID {request.InvitedBy.Value} does not exist");
            }

            // Create invitation
            var invitation = new BoardInvitation
            {
                Id = Guid.NewGuid(),
                BoardId = request.BoardId,
                UserId = request.UserId,
                InvitedBy = request.InvitedBy.Value,
                Role = request.Role,
                Status = InvitationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.BoardInvitations.AddAsync(invitation, cancellationToken);

            board.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Boards.Update(board);

            // Send invitation email
            var emailBody = $@"
                <p>Hello {user.Username ?? user.Email},</p>
                <p><strong>{inviter.Username ?? inviter.Email}</strong> has invited you to join the board <strong>{board.Name}</strong> as a <strong>{request.Role}</strong>.</p>
                <p>Please log in to TaskPilot to accept or decline this invitation.</p>
                <p>Best regards,<br/>TaskPilot Team</p>";

            await _emailService.SendSystemEmailAsync(
                user.Email,
                $"Board Invitation: {board.Name}",
                emailBody,
                cancellationToken);
        }, cancellationToken);
    }
}
