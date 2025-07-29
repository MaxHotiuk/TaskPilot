using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Meetings;
using MediatR;

namespace Application.Queries.Meetings;

public class GetMeetingsByBoardIdQueryHandler : BaseQueryHandler, IRequestHandler<GetMeetingsByBoardIdQuery, IEnumerable<MeetingDto>>
{
    public GetMeetingsByBoardIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory) { }

    public async Task<IEnumerable<MeetingDto>> Handle(GetMeetingsByBoardIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.Meetings.GetMeetingsByBoardIdAsync(request.BoardId, cancellationToken);
        }, cancellationToken);
    }
}
