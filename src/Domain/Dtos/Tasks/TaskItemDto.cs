namespace Domain.Dtos.Tasks;

public record TaskItemDto
{
    public Guid Id { get; init; }
    public Guid BoardId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int StateId { get; init; }
    public Guid? AssigneeId { get; init; }
    public int? TagId { get; init; }
    public int Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
