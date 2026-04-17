using MediatR;

namespace Application.Commands.GoogleCalendar;

/// <summary>
/// Syncs all meetings and tasks for the given month to the user's Google Calendar.
/// </summary>
public record SyncGoogleCalendarCommand(
    Guid UserId,
    DateTime Month) : IRequest;
