using MediatR;

namespace Application.Commands.Comments;

public record CreateCommentCommand(
    Guid TaskId,
    Guid AuthorId,
    string Content
) : IRequest<Guid>;
