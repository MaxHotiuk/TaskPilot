using MediatR;

namespace Application.Commands.GoogleCalendar;

/// <summary>
/// Request body sent by the Blazor frontend after it captures the OAuth2
/// authorization code from Google's redirect.
/// </summary>
public record ConnectGoogleCalendarRequest(string Code);

/// <summary>
/// Exchanges the OAuth2 authorization code received from Google's redirect for
/// access/refresh tokens and persists them against the user record.
/// </summary>
public record ConnectGoogleCalendarCommand(
    Guid UserId,
    string Code) : IRequest;
