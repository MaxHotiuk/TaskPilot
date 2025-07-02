using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Comments;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Guid>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCommentCommandHandler(
        ICommentRepository commentRepository,
        ITaskItemRepository taskItemRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _taskItemRepository = taskItemRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        // Validate task exists
        var task = await _taskItemRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
        {
            throw new ValidationException($"Task with ID {request.TaskId} does not exist");
        }

        // Validate author exists
        var author = await _userRepository.GetByIdAsync(request.AuthorId, cancellationToken);
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

        await _commentRepository.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}
