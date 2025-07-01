namespace Domain.Entities;

public class Comment : IEntity<Guid>, IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public TaskItem Task { get; set; } = null!;
    public User Author { get; set; } = null!;
}
