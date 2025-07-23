using Application.Abstractions.Persistence;
using Domain.Dtos.Tags;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tags;

public class GetTagsByBoardIdQueryHandler : BaseQueryHandler, IRequestHandler<GetTagsByBoardIdQuery, IEnumerable<TagDto>>
{
    public GetTagsByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<TagDto>> Handle(GetTagsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var tags = await unitOfWork.Tags.GetTagsByBoardIdAsync(request.BoardId, cancellationToken);
            return tags.ToDto().OrderBy(t => t.Name);
        }, cancellationToken);
    }
}
