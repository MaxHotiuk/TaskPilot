using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.Comments;

public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentCommandHandler(ICommentRepository commentRepository, IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (comment is null)
        {
            throw new NotFoundException($"Comment with ID {request.Id} was not found");
        }

        _commentRepository.Remove(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
