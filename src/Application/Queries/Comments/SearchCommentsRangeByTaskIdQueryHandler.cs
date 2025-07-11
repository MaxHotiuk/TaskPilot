using Application.Abstractions.Persistence;
using Application.Common.Dtos.Comments;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Comments;

public class SearchCommentsRangeByTaskIdQueryHandler : BaseQueryHandler, IRequestHandler<SearchCommentsRangeByTaskIdQuery, IEnumerable<CommentDto>>
{
    public SearchCommentsRangeByTaskIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<CommentDto>> Handle(SearchCommentsRangeByTaskIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var comments = await unitOfWork.Comments.SearchCommentsRangeByTaskIdAsync(
                request.SearchTerm, request.TaskId, request.Page, request.PageSize, cancellationToken);
            return comments.ToDto().OrderBy(c => c.CreatedAt);
        }, cancellationToken);
    }
}
