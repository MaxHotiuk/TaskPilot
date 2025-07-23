namespace Domain.Entities;

public class Tag : AuditableEntity<int>
{
    public Guid BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }

    // Navigation properties
    public Board Board { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
