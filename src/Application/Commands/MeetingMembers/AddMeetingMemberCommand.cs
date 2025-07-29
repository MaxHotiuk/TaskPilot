using MediatR;
using System;

namespace Application.Commands.MeetingMembers;

public record AddMeetingMemberCommand(Guid MeetingId, Guid UserId) : IRequest;
