using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Dtos.Meetings;
using MediatR;

namespace Application.Queries.Meetings;

public class GetMeetingCalendarItemsQueryHandler : BaseQueryHandler, IRequestHandler<GetMeetingCalendarItemsQuery, IEnumerable<MeetingCalendarItemDto>>
{
    public GetMeetingCalendarItemsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory) { }

    public async Task<IEnumerable<MeetingCalendarItemDto>> Handle(GetMeetingCalendarItemsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.Meetings.GetMeetingCalendarItemsAsync(
                request.UserId,
                request.StartDate,
                request.EndDate,
                cancellationToken);
        }, cancellationToken);
    }
}
