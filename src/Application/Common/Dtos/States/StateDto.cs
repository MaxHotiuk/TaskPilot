namespace Application.Common.Dtos.States;

public record StateDto
{
    public int Id { get; init; }
    public Guid BoardId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
