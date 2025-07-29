using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Meetings;
using MediatR;

namespace Application.Queries.Meetings;

public class GetMeetingsByUserIdQueryHandler : BaseQueryHandler, IRequestHandler<GetMeetingsByUserIdQuery, IEnumerable<MeetingDto>>
{
    public GetMeetingsByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory) { }

    public async Task<IEnumerable<MeetingDto>> Handle(GetMeetingsByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.Meetings.GetMeetingsByUserIdAsync(request.UserId, cancellationToken);
        }, cancellationToken);
    }
}
