using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Meetings;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Meetings;

public sealed class DailyRoomService : IDailyRoomService
{
    private readonly HttpClient _httpClient;
    private readonly DailyOptions _options;

    public DailyRoomService(HttpClient httpClient, IOptions<DailyOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> CreateRoomAsync(Guid meetingId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Daily API key is not configured");
        }

        var request = new DailyCreateRoomRequest
        {
            Name = $"{_options.RoomNamePrefix}{meetingId:N}",
            Properties = new DailyRoomProperties
            {
                EnableChat = true
            }
        };

        using var response = await _httpClient.PostAsJsonAsync("rooms", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<DailyCreateRoomResponse>(cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(payload?.Url))
        {
            throw new InvalidOperationException("Daily API response did not include a room URL");
        }

        return payload.Url;
    }

    private sealed class DailyCreateRoomRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("properties")]
        public DailyRoomProperties Properties { get; init; } = new();
    }

    private sealed class DailyRoomProperties
    {
        [JsonPropertyName("enable_chat")]
        public bool EnableChat { get; init; }
    }

    private sealed class DailyCreateRoomResponse
    {
        [JsonPropertyName("url")]
        public string? Url { get; init; }
    }
}
