using Application.Abstractions.Persistence;
using Application.Common.Dtos.Comments;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Comments;

public class GetCommentsByTaskIdQueryHandler : IRequestHandler<GetCommentsByTaskIdQuery, IEnumerable<CommentDto>>
{
    private readonly ICommentRepository _commentRepository;

    public GetCommentsByTaskIdQueryHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<IEnumerable<CommentDto>> Handle(GetCommentsByTaskIdQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentRepository.GetCommentsByTaskIdAsync(request.TaskId, cancellationToken);
        return comments.ToDto().OrderBy(c => c.CreatedAt);
    }
}
