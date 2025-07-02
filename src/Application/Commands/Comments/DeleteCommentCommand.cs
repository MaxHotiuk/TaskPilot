using MediatR;

namespace Application.Commands.Comments;

public record DeleteCommentCommand(Guid Id) : IRequest;
