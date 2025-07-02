using Application.Abstractions.Persistence;
using Application.Common.Dtos.Comments;
using MediatR;

namespace Application.Queries.Comments;

public class GetCommentByIdQueryHandler : IRequestHandler<GetCommentByIdQuery, CommentDto?>
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentByIdQueryHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<CommentDto?> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (comment is null)
        {
            return null;
        }

        return new CommentDto
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}
