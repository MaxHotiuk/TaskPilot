using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Comments;

public class UpdateCommentCommandHandler : BaseCommandHandler, IRequestHandler<UpdateCommentCommand>
{

    private readonly IBoardNotifier _boardNotifier;

    public UpdateCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var comment = await unitOfWork.Comments.GetByIdAsync(request.Id, cancellationToken);
            
            if (comment is null)
            {
                throw new NotFoundException($"Comment with ID {request.Id} was not found");
            }

            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Comments.Update(comment);

            var task = await unitOfWork.Tasks.GetByIdAsync(comment.TaskId, cancellationToken);
            await unitOfWork.Boards.TouchBoardAsync(task!.BoardId, cancellationToken);

            await _boardNotifier.NotifyTaskUpdatedAsync(comment.TaskId.ToString(), new { action = "commentUpdated", commentId = comment.Id });
        }, cancellationToken);
    }
}
