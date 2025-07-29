using MediatR;
using Domain.Dtos.Meetings;
using System;
using System.Collections.Generic;

namespace Application.Queries.Meetings;

public record GetMeetingCalendarItemsQuery(
    Guid UserId,
    DateTime StartDate,
    DateTime EndDate
) : IRequest<IEnumerable<MeetingCalendarItemDto>>;
