using MediatR;
using Domain.Dtos.Meetings;
using System;
using System.Collections.Generic;

namespace Application.Queries.MeetingMembers;

public record GetMeetingMembersByMeetingIdQuery(Guid MeetingId) : IRequest<IEnumerable<MeetingMemberDto>>;
