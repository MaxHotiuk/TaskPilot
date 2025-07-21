using Application.Abstractions.Persistence;
using Domain.Dtos.BoardMembers;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.BoardMembers;

public class GetBoardMembersByUserIdQueryHandler : BaseQueryHandler, IRequestHandler<GetBoardMembersByUserIdQuery, IEnumerable<BoardMemberDto>>
{
    public GetBoardMembersByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<BoardMemberDto>> Handle(GetBoardMembersByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var boardMembers = await unitOfWork.BoardMembers.GetBoardsByUserIdAsync(request.UserId, cancellationToken);
            return boardMembers.ToDto();
        }, cancellationToken);
    }
}
