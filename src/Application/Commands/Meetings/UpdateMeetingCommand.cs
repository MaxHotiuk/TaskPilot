using MediatR;
using System;

namespace Application.Commands.Meetings;

public record UpdateMeetingCommand(
    Guid Id,
    string Title,
    string Description,
    DateTime? ScheduledAt,
    int? Duration
) : IRequest;
