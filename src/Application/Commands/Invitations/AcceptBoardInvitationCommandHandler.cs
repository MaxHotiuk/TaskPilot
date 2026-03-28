using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Invitations;

public class AcceptBoardInvitationCommandHandler : BaseCommandHandler, IRequestHandler<AcceptBoardInvitationCommand>
{
    private readonly IBoardNotifier _boardNotifier;
    private readonly INotificationNotifier _notificationNotifier;

    public AcceptBoardInvitationCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBoardNotifier boardNotifier,
        INotificationNotifier notificationNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _notificationNotifier = notificationNotifier;
    }

    public async Task Handle(AcceptBoardInvitationCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var invitation = await unitOfWork.BoardInvitations.GetByIdAsync(request.InvitationId, cancellationToken);
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

            var board = await unitOfWork.Boards.GetByIdAsync(invitation.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {invitation.BoardId} does not exist");
            }

            var user = await unitOfWork.Users.GetByIdAsync(invitation.UserId, cancellationToken);
            if (user is null)
            {
                throw new ValidationException($"User with ID {invitation.UserId} does not exist");
            }

            // Check if user is already a member
            if (await unitOfWork.BoardMembers.IsMemberOfBoardAsync(invitation.BoardId, invitation.UserId, cancellationToken))
            {
                throw new ValidationException($"User is already a member of board {invitation.BoardId}");
            }

            // Update invitation status
            invitation.Status = InvitationStatus.Accepted;
            invitation.RespondedAt = DateTime.UtcNow;
            invitation.UpdatedAt = DateTime.UtcNow;
            unitOfWork.BoardInvitations.Update(invitation);

            // Add user to board
            var boardMember = new BoardMember
            {
                BoardId = invitation.BoardId,
                UserId = invitation.UserId,
                Role = invitation.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.BoardMembers.AddAsync(boardMember, cancellationToken);

            // Add to board chat if exists
            var boardChat = await unitOfWork.Chats.GetBoardChatAsync(invitation.BoardId, cancellationToken);
            if (boardChat is not null)
            {
                var existingChatMember = await unitOfWork.ChatMembers.GetMemberAsync(boardChat.Id, invitation.UserId, cancellationToken);
                if (existingChatMember is null)
                {
                    var chatMember = new ChatMember
                    {
                        ChatId = boardChat.Id,
                        UserId = invitation.UserId,
                        Role = ChatMemberRole.Member,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await unitOfWork.ChatMembers.AddAsync(chatMember, cancellationToken);
                    boardChat.UpdatedAt = DateTime.UtcNow;
                    unitOfWork.Chats.Update(boardChat);
                }
            }

            // Create backlog entry
            var backlogEntry = new Backlog
            {
                BoardId = invitation.BoardId,
                Description = $"User '{user.Username ?? user.Id.ToString()}' joined the board as '{invitation.Role}'."
            };
            await unitOfWork.Backlogs.AddAsync(backlogEntry, cancellationToken);

            // Create notification
            var notification = unitOfWork.Notifications.BuildNotification(
                userId: invitation.UserId,
                type: Domain.Enums.NotificationType.AddedToBoard,
                boardId: invitation.BoardId,
                boardName: board.Name
            );
            await unitOfWork.Notifications.AddAsync(notification, cancellationToken);

            await _notificationNotifier.NotifyUserAsync(invitation.UserId, notification);
            await _boardNotifier.NotifyBoardUpdatedAsync(boardMember.BoardId.ToString(), new { action = "addedUser", boardId = boardMember.BoardId });
        }, cancellationToken);
    }
}
