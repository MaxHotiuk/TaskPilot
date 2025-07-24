namespace Domain.Entities;

public class Board : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchivedAt { get; set; }
    public string? ArchivalReason { get; set; }

    // Navigation properties
    public User Owner { get; set; } = null!;
    public ICollection<State> States { get; set; } = new List<State>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();
    public ICollection<ArchivalJob> ArchivalJobs { get; set; } = new List<ArchivalJob>();
    public ICollection<Backlog> Backlog { get; set; } = new List<Backlog>();
}