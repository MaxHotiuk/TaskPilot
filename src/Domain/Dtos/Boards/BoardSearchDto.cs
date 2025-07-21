namespace Domain.Dtos.Boards;

public record BoardSearchDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int NumberOfMembers { get; init; }
    public int NumberOfTasks { get; init; }
    public Guid OwnerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
