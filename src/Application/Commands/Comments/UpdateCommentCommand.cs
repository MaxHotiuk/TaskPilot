using MediatR;

namespace Application.Commands.Comments;

public record UpdateCommentCommand(
    Guid Id,
    string Content
) : IRequest;
