using Application.Abstractions.Persistence;
using Domain.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Boards;

public class GetBoardsByUserIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBoardsByUserIdQuery, IEnumerable<BoardDto>>
{
    public GetBoardsByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardDto>> Handle(GetBoardsByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boards = await unitOfWork.Boards.GetBoardsByUserIdAsync(request.UserId, cancellationToken);
            return boards.ToDto();
        }, cancellationToken);
    }
}
