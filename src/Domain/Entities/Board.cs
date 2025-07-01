namespace Domain.Entities;

public class Board : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }

    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<State> States { get; set; } = new List<State>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();
}
