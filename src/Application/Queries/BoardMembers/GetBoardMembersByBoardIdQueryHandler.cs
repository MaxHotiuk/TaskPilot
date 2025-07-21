using Application.Abstractions.Persistence;
using Domain.Dtos.BoardMembers;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.BoardMembers;

public class GetBoardMembersByBoardIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBoardMembersByBoardIdQuery, IEnumerable<BoardMemberDto>>
{
    public GetBoardMembersByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardMemberDto>> Handle(GetBoardMembersByBoardIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boardMembers = await unitOfWork.BoardMembers.GetMembersByBoardIdAsync(request.BoardId, cancellationToken);
            return boardMembers.ToDto();
        }, cancellationToken);
    }
}
