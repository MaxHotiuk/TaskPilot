using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Chats;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Tasks;

public class CreateTaskItemCommandHandler : BaseCommandHandler, IRequestHandler<CreateTaskItemCommand, Guid>
{

    private readonly IBoardNotifier _boardNotifier;
    private readonly IChatNotifier _chatNotifier;
    private readonly INotificationNotifier _notificationNotifier;

    public CreateTaskItemCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier, IChatNotifier chatNotifier, INotificationNotifier notificationNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _chatNotifier = chatNotifier;
        _notificationNotifier = notificationNotifier;
    }

    public async Task<Guid> Handle(CreateTaskItemCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.BoardId, cancellationToken);
            if (board is null)
            {
                throw new ValidationException($"Board with ID {request.BoardId} does not exist");
            }

            if (!await unitOfWork.States.IsValidStateForBoardAsync(request.StateId, request.BoardId, cancellationToken))
            {
                throw new ValidationException($"State with ID {request.StateId} is not valid for board {request.BoardId}");
            }

            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = request.BoardId,
                Title = request.Title,
                Description = request.Description,
                StateId = request.StateId,
                TagId = request.TagId,
                Priority = request.Priority,
                AssigneeId = request.AssigneeId,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Tasks.AddAsync(taskItem, cancellationToken);

            board.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Boards.Update(board);

            var backlogEntry = Application.Common.Helpers.BacklogEntryHelper.CreateBacklogForTaskCreate(taskItem, unitOfWork.States, unitOfWork.Users);
            await unitOfWork.Backlogs.AddAsync(backlogEntry, cancellationToken);

            if (request.AssigneeId != null)
            {
                var notification = unitOfWork.Notifications.BuildNotification(
                    userId: request.AssigneeId.Value,
                    type: Domain.Enums.NotificationType.AssignedToTask,
                    taskId: taskItem.Id,
                    boardId: request.BoardId,
                    taskName: taskItem.Title
                );
                await unitOfWork.Notifications.AddAsync(notification, cancellationToken);
                await _notificationNotifier.NotifyUserAsync(request.AssigneeId.Value, notification);
            }

            var boardChat = await unitOfWork.Chats.GetBoardChatAsync(request.BoardId, cancellationToken);
            if (boardChat is not null)
            {
                var sender = await unitOfWork.Users.GetByIdAsync(board.OwnerId, cancellationToken);
                if (sender is not null)
                {
                    var message = new ChatMessage
                    {
                        Id = Guid.NewGuid(),
                        ChatId = boardChat.Id,
                        SenderId = sender.Id,
                        Sender = sender,
                        TaskId = taskItem.Id,
                        AssigneeId = taskItem.AssigneeId,
                        Content = $"Task created: {taskItem.Title}",
                        MessageType = "Task",
                        HasAttachments = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await unitOfWork.ChatMessages.AddAsync(message, cancellationToken);

                    boardChat.UpdatedAt = DateTime.UtcNow;
                    unitOfWork.Chats.Update(boardChat);

                    var messageDto = message.ToDto();
                    var memberIds = await unitOfWork.ChatMembers.GetMemberIdsAsync(boardChat.Id, cancellationToken);
                    var chatMembers = await unitOfWork.ChatMembers.GetMembersAsync(boardChat.Id, cancellationToken);
                    var chatDto = new ChatDto
                    {
                        Id = boardChat.Id,
                        OrganizationId = boardChat.OrganizationId,
                        BoardId = boardChat.BoardId,
                        Name = boardChat.Name,
                        Type = boardChat.Type,
                        CreatedById = boardChat.CreatedById,
                        CreatedAt = boardChat.CreatedAt,
                        UpdatedAt = boardChat.UpdatedAt,
                        LastMessage = message.ToPreviewDto(),
                        Members = chatMembers.Select(member => member.ToDto()).ToList()
                    };

                    await _chatNotifier.NotifyChatMessageAsync(boardChat.Id, messageDto);
                    await _chatNotifier.NotifyChatUpdatedAsync(memberIds, chatDto);
                }
            }

            await _boardNotifier.NotifyBoardUpdatedAsync(request.BoardId.ToString(), new { action = "created", boardId = request.BoardId });
            return taskItem.Id;
        }, cancellationToken);
    }
}
