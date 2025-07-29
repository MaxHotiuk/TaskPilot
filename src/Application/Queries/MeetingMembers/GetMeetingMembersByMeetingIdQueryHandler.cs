using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Meetings;
using MediatR;

namespace Application.Queries.MeetingMembers;

public class GetMeetingMembersByMeetingIdQueryHandler : BaseQueryHandler, IRequestHandler<GetMeetingMembersByMeetingIdQuery, IEnumerable<MeetingMemberDto>>
{
    public GetMeetingMembersByMeetingIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory) { }

    public async Task<IEnumerable<MeetingMemberDto>> Handle(GetMeetingMembersByMeetingIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var members = await unitOfWork.MeetingMembers.GetMembersByMeetingIdAsync(request.MeetingId, cancellationToken);
            return members.Select(m => m.ToDto());
        }, cancellationToken);
    }
}
