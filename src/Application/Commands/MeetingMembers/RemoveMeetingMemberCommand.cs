using MediatR;
using System;

namespace Application.Commands.MeetingMembers;

public record RemoveMeetingMemberCommand(Guid MeetingId, Guid UserId) : IRequest;
