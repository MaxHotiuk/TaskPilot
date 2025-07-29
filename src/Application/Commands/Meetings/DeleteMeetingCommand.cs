using MediatR;
using System;

namespace Application.Commands.Meetings;

public record DeleteMeetingCommand(Guid Id) : IRequest;
