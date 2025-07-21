using Domain.Dtos.Comments;
using MediatR;

namespace Application.Queries.Comments;

public record SearchCommentsRangeByTaskIdQuery(string SearchTerm, Guid TaskId, int Page, int PageSize) : IRequest<IEnumerable<CommentDto>>;
