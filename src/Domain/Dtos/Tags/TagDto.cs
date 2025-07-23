namespace Domain.Dtos.Tags;

public record TagDto
{
    public int Id { get; init; }
    public Guid BoardId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Color { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
