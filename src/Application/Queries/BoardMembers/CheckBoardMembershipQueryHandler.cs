using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using MediatR;

namespace Application.Queries.BoardMembers;

public class CheckBoardMembershipQueryHandler : BaseQueryHandler, IRequestHandler<CheckBoardMembershipQuery, bool>
{
    public CheckBoardMembershipQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<bool> Handle(CheckBoardMembershipQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.BoardMembers.IsMemberOfBoardAsync(request.BoardId, request.UserId, cancellationToken);
        }, cancellationToken);
    }
}
