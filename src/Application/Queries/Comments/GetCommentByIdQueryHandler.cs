using Application.Abstractions.Persistence;
using Domain.Dtos.Comments;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Comments;

public class GetCommentByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetCommentByIdQuery, CommentDto?>
{
    public GetCommentByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<CommentDto?> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var comment = await unitOfWork.Comments.GetByIdAsync(request.Id, cancellationToken);
            return comment?.ToDto();
        }, cancellationToken);
    }
}
