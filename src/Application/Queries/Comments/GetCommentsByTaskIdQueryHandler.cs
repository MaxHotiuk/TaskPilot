using Application.Abstractions.Persistence;
using Domain.Dtos.Comments;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Comments;

public class GetCommentsByTaskIdQueryHandler : BaseQueryHandler, IRequestHandler<GetCommentsByTaskIdQuery, IEnumerable<CommentDto>>
{
    public GetCommentsByTaskIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<CommentDto>> Handle(GetCommentsByTaskIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var comments = await unitOfWork.Comments.GetCommentsByTaskIdAsync(request.TaskId, cancellationToken);
            return comments.ToDto().OrderBy(c => c.CreatedAt);
        }, cancellationToken);
    }
}
