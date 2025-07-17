using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Comments;

public class CreateCommentCommandHandler : BaseCommandHandler, IRequestHandler<CreateCommentCommand, Guid>
{

    private readonly IBoardNotifier _boardNotifier;

    public CreateCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
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
            
            await _boardNotifier.NotifyTaskUpdatedAsync(comment.TaskId.ToString(), new { action = "commentCreated", commentId = comment.Id });
            return comment.Id;
        }, cancellationToken);
    }
}
