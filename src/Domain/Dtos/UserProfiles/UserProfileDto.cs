namespace Domain.Dtos.UserProfiles;

public record UserProfileDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string? Bio { get; init; }
    public string? JobTitle { get; init; }
    public string? Department { get; init; }
    public string? Location { get; init; }
    public string? PhoneNumber { get; init; }
    public bool AddToBoardAutomatically { get; init; }
    public bool ShowEmail { get; init; }
    public bool ShowPhoneNumber { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
