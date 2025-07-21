using Domain.Dtos.Comments;
using MediatR;

namespace Application.Queries.Comments;

public record GetCommentsByTaskIdQuery(Guid TaskId) : IRequest<IEnumerable<CommentDto>>;
