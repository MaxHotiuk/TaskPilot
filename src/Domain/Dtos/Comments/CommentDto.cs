namespace Domain.Dtos.Comments;

public record CommentDto
{
    public Guid Id { get; init; }
    public Guid TaskId { get; init; }
    public Guid AuthorId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
