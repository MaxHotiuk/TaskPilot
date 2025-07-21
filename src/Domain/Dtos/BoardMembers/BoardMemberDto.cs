namespace Domain.Dtos.BoardMembers;

public record BoardMemberDto
{
    public Guid BoardId { get; init; }
    public Guid UserId { get; init; }
    public string Role { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
