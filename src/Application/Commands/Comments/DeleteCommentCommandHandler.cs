using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Comments;

public class DeleteCommentCommandHandler : BaseCommandHandler, IRequestHandler<DeleteCommentCommand>
{
    public DeleteCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
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
        }, cancellationToken);
    }
}
