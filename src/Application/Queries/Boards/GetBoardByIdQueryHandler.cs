using Application.Abstractions.Persistence;
using Application.Common.Dtos.Boards;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Boards;

public class GetBoardByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBoardByIdQuery, BoardDto?>
{
    public GetBoardByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<BoardDto?> Handle(GetBoardByIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var board = await unitOfWork.Boards.GetByIdAsync(request.Id, cancellationToken);
            return board?.ToDto();
        }, cancellationToken);
    }
}
