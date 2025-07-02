using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Commands.Comments;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommentCommandHandler(ICommentRepository commentRepository, IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (comment is null)
        {
            throw new NotFoundException($"Comment with ID {request.Id} was not found");
        }

        comment.Content = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;

        _commentRepository.Update(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
