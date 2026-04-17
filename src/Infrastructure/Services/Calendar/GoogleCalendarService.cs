using Application.Abstractions.Calendar;
using Domain.Dtos.Meetings;
using Domain.Dtos.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Calendar;

public sealed class GoogleCalendarService : IGoogleCalendarService
{
    private const string ApplicationName = "TaskPilot";
    private const string PrimaryCalendar = "primary";

    private readonly GoogleCalendarOptions _options;
    private readonly ILogger<GoogleCalendarService> _logger;

    public GoogleCalendarService(
        IOptions<GoogleCalendarOptions> options,
        ILogger<GoogleCalendarService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    // -------------------------------------------------------------------------
    // Auth helpers
    // -------------------------------------------------------------------------

    public string GetAuthorizationUrl(string state)
    {
        var flow = CreateFlow();

        return flow.CreateAuthorizationCodeRequest(_options.RedirectUri)
            .Build()
            .AbsoluteUri
            + $"&state={Uri.EscapeDataString(state)}";
    }

    public async Task<GoogleTokenResult> ExchangeCodeForTokensAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var flow = CreateFlow();

        var tokenResponse = await flow.ExchangeCodeForTokenAsync(
            userId: string.Empty,
            code: code,
            redirectUri: _options.RedirectUri,
            taskCancellationToken: cancellationToken);

        var expiry = tokenResponse.IssuedUtc.AddSeconds(tokenResponse.ExpiresInSeconds ?? 3600);

        return new GoogleTokenResult(
            tokenResponse.AccessToken,
            tokenResponse.RefreshToken ?? string.Empty,
            expiry);
    }

    // -------------------------------------------------------------------------
    // Sync operations
    // -------------------------------------------------------------------------

    public async Task SyncMeetingsAsync(
        string accessToken,
        string refreshToken,
        IEnumerable<MeetingCalendarItemDto> meetings,
        CancellationToken cancellationToken = default)
    {
        using var service = CreateCalendarService(accessToken, refreshToken);

        foreach (var meeting in meetings.Where(m => m.ScheduledAt.HasValue))
        {
            var iCalUid = BuildICalUid("meeting", meeting.Id);

            try
            {
                var startTime = meeting.ScheduledAt!.Value;
                var endTime = meeting.Duration.HasValue
                    ? startTime.AddMinutes(meeting.Duration.Value)
                    : startTime.AddHours(1);

                var calendarEvent = new Event
                {
                    Summary = meeting.Title,
                    Description = meeting.Description,
                    ICalUID = iCalUid,
                    Start = new EventDateTime { DateTimeDateTimeOffset = startTime, TimeZone = "UTC" },
                    End = new EventDateTime { DateTimeDateTimeOffset = endTime, TimeZone = "UTC" },
                    Source = new Event.SourceData
                    {
                        Title = ApplicationName,
                        Url = $"https://taskpilot.app/boards/{meeting.BoardId}"
                    }
                };

                await UpsertEventAsync(service, PrimaryCalendar, iCalUid, calendarEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to sync meeting {MeetingId} to Google Calendar", meeting.Id);
            }
        }
    }

    public async Task SyncTasksAsync(
        string accessToken,
        string refreshToken,
        IEnumerable<TaskCalendarItemDto> tasks,
        CancellationToken cancellationToken = default)
    {
        using var service = CreateCalendarService(accessToken, refreshToken);

        foreach (var task in tasks.Where(t => t.DueDate.HasValue))
        {
            var iCalUid = BuildICalUid("task", task.Id);

            try
            {
                var dueDate = task.DueDate!.Value.Date;

                var calendarEvent = new Event
                {
                    Summary = $"[Task] {task.Title}",
                    ICalUID = iCalUid,
                    // All-day event: supply Date (not DateTime)
                    Start = new EventDateTime { Date = dueDate.ToString("yyyy-MM-dd") },
                    End = new EventDateTime { Date = dueDate.AddDays(1).ToString("yyyy-MM-dd") },
                    Source = new Event.SourceData
                    {
                        Title = ApplicationName,
                        Url = $"https://taskpilot.app/boards/{task.BoardId}"
                    }
                };

                await UpsertEventAsync(service, PrimaryCalendar, iCalUid, calendarEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to sync task {TaskId} to Google Calendar", task.Id);
            }
        }
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private GoogleAuthorizationCodeFlow CreateFlow() =>
        new(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            },
            Scopes = [CalendarService.Scope.Calendar]
        });

    private CalendarService CreateCalendarService(string accessToken, string refreshToken)
    {
        var tokenResponse = new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        var credential = new UserCredential(
            CreateFlow(),
            userId: string.Empty,
            tokenResponse);

        return new CalendarService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        });
    }

    /// <summary>
    /// Upserts a calendar event using iCalUID-based lookup.
    /// If an event with the same iCalUID already exists it is patched; otherwise a new event is inserted.
    /// </summary>
    private async Task UpsertEventAsync(
        CalendarService service,
        string calendarId,
        string iCalUid,
        Event calendarEvent,
        CancellationToken cancellationToken)
    {
        var listRequest = service.Events.List(calendarId);
        listRequest.ICalUID = iCalUid;
        listRequest.MaxResults = 1;

        var existingEvents = await listRequest.ExecuteAsync(cancellationToken);
        var existing = existingEvents.Items?.FirstOrDefault();

        if (existing is not null)
        {
            await service.Events
                .Patch(calendarEvent, calendarId, existing.Id)
                .ExecuteAsync(cancellationToken);
        }
        else
        {
            await service.Events
                .Insert(calendarEvent, calendarId)
                .ExecuteAsync(cancellationToken);
        }
    }

    private static string BuildICalUid(string entityType, Guid id) =>
        $"taskpilot-{entityType}-{id:N}@taskpilot.app";
}
