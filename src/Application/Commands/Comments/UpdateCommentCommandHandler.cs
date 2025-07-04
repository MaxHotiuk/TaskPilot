using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Comments;

public class UpdateCommentCommandHandler : BaseCommandHandler, IRequestHandler<UpdateCommentCommand>
{
    public UpdateCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
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
        }, cancellationToken);
    }
}
