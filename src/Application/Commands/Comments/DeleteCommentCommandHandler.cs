using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Comments;

public class DeleteCommentCommandHandler : BaseCommandHandler, IRequestHandler<DeleteCommentCommand>
{

    private readonly IBoardNotifier _boardNotifier;

    public DeleteCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IBoardNotifier boardNotifier)
        : base(unitOfWorkFactory)
    {
        _boardNotifier = boardNotifier;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var comment = await unitOfWork.Comments.GetByIdAsync(request.Id, cancellationToken);
            
            if (comment is null)
            {
                throw new NotFoundException($"Comment with ID {request.Id} was not found");
            }

            unitOfWork.Comments.Remove(comment);

            await _boardNotifier.NotifyTaskUpdatedAsync(comment.TaskId.ToString(), new { action = "commentDeleted", commentId = comment.Id });
        }, cancellationToken);
    }
}
