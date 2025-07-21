using Domain.Dtos.Comments;
using MediatR;

namespace Application.Queries.Comments;

public record GetCommentByIdQuery(Guid Id) : IRequest<CommentDto?>;
