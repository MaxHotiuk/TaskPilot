using Domain.Dtos.Meetings;
using Domain.Dtos.Tasks;

namespace Application.Abstractions.Calendar;

/// <summary>
/// Result returned after a successful Google OAuth2 token exchange.
/// </summary>
public record GoogleTokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime Expiry);

public interface IGoogleCalendarService
{
    /// <summary>
    /// Builds the Google OAuth2 authorization URL that the user's browser must be redirected to.
    /// </summary>
    /// <param name="state">An opaque value forwarded by Google and validated on callback to prevent CSRF.</param>
    string GetAuthorizationUrl(string state);

    /// <summary>
    /// Exchanges the short-lived authorization <paramref name="code"/> (received from Google's redirect)
    /// for a long-lived access token and refresh token.
    /// </summary>
    Task<GoogleTokenResult> ExchangeCodeForTokensAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upserts the supplied <paramref name="meetings"/> as events on the user's primary Google Calendar.
    /// Uses a deterministic iCalUID derived from the meeting ID so repeated syncs are idempotent.
    /// </summary>
    Task SyncMeetingsAsync(
        string accessToken,
        string refreshToken,
        IEnumerable<MeetingCalendarItemDto> meetings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Upserts the supplied <paramref name="tasks"/> (those that have a due date) as all-day events
    /// on the user's primary Google Calendar.
    /// </summary>
    Task SyncTasksAsync(
        string accessToken,
        string refreshToken,
        IEnumerable<TaskCalendarItemDto> tasks,
        CancellationToken cancellationToken = default);
}
