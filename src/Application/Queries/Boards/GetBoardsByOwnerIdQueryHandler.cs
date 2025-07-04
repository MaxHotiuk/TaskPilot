using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Boards;

public class GetBoardsByOwnerIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBoardsByOwnerIdQuery, IEnumerable<BoardDto>>
{
    public GetBoardsByOwnerIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetBoardsByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.GetBoardsByOwnerIdAsync(request.OwnerId, cancellationToken);
            return boards.ToDto();
        }, cancellationToken);
    }
}
