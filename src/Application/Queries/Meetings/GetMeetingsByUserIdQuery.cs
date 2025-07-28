using MediatR;
using Domain.Dtos.Meetings;
using System;
using System.Collections.Generic;

namespace Application.Queries.Meetings;

public record GetMeetingsByUserIdQuery(Guid UserId) : IRequest<IEnumerable<MeetingDto>>;
