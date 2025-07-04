using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Comments;

public class CreateCommentCommandHandler : BaseCommandHandler, IRequestHandler<CreateCommentCommand, Guid>
{
    public CreateCommentCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
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
            
            return comment.Id;
        }, cancellationToken);
    }
}
