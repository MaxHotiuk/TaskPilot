using Domain.Dtos.Organizations;

namespace Domain.Dtos.Users;

public record CurrentUserDto
{
    public Guid Id { get; init; }
    public string EntraId { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsGoogleCalendarConnected { get; init; }
    public IEnumerable<OrganizationSummaryDto> Organizations { get; init; } = new List<OrganizationSummaryDto>();
}
