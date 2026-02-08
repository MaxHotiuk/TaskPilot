namespace Infrastructure.Services.Meetings;

public sealed class DailyOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string ApiBaseUrl { get; init; } = "https://api.daily.co/v1/";
    public string RoomNamePrefix { get; init; } = "taskpilot-";
}
