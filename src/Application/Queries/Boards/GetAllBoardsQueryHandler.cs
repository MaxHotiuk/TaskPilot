using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Boards;

public class GetAllBoardsQueryHandler : BaseQueryHandler, IRequestHandler<GetAllBoardsQuery, IEnumerable<BoardDto>>
{
    public GetAllBoardsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetAllBoardsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.GetAllAsync(cancellationToken);
            return boards.ToDto();
        }, cancellationToken);
    }
}
