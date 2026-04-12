using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;

namespace Application.Queries.Boards;

public class GetStaleBoardIdsQueryHandler : BaseQueryHandler, IRequestHandler<GetStaleBoardIdsQuery, IEnumerable<Guid>>
{
    public GetStaleBoardIdsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<Guid>> Handle(GetStaleBoardIdsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.Boards.GetStaleBoardIdsAsync(request.StaleDays, cancellationToken);
        }, cancellationToken);
    }
}
