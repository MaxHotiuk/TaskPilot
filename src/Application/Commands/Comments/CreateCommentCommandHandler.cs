using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Commands.Notifications;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Comments;

public class CreateCommentCommandHandler : BaseCommandHandler, IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly IBoardNotifier _boardNotifier;
    private readonly ISender _sender;

    public CreateCommentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IBoardNotifier boardNotifier,
        ISender sender)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
        _sender = sender;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        Guid? notificationAssigneeId = null;
        Guid notificationTaskId = default;
        Guid notificationBoardId = default;
        string notificationTaskName = string.Empty;
        string notificationComment = string.Empty;
        Guid commentId = default;

        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var task = await unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
            if (task is null)
            {
                throw new ValidationException($"Task with ID {request.TaskId} does not exist");
            }

            var author = await unitOfWork.Users.GetByIdAsync(request.AuthorId, cancellationToken);
            if (author is null)
            {
                throw new ValidationException($"User with ID {request.AuthorId} does not exist");
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                TaskId = request.TaskId,
                AuthorId = request.AuthorId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Comments.AddAsync(comment, cancellationToken);

            if (task.AssigneeId != null && task.AssigneeId != request.AuthorId)
            {
                notificationAssigneeId = task.AssigneeId;
                notificationTaskId = task.Id;
                notificationBoardId = task.BoardId;
                notificationTaskName = task.Title;
                notificationComment = comment.Content;
            }

            await unitOfWork.Boards.TouchBoardAsync(task.BoardId, cancellationToken);

            await _boardNotifier.NotifyTaskUpdatedAsync(comment.TaskId.ToString(), new { action = "commentCreated", commentId = comment.Id });
            commentId = comment.Id;
        }, cancellationToken);

        if (notificationAssigneeId.HasValue)
        {
            await _sender.Send(new CreateNotificationCommand(
                UserId: notificationAssigneeId.Value,
                Type: NotificationType.CommentedOnTask,
                TaskId: notificationTaskId,
                BoardId: notificationBoardId,
                TaskName: notificationTaskName,
                UserComment: notificationComment
            ), cancellationToken);
        }

        return commentId;
    }
}
