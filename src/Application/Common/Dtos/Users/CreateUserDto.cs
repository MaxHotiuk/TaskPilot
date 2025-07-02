namespace Application.Common.Dtos.Users;

public record CreateUserDto
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string EntraId { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
