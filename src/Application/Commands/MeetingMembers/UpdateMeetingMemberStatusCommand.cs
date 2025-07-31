using MediatR;
using System;

namespace Application.Commands.MeetingMembers;

public record UpdateMeetingMemberStatusCommand(Guid MeetingId, Guid UserId, string Status) : IRequest;
