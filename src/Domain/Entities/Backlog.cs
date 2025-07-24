namespace Domain.Entities;

public class Backlog : Entity<Guid>
{
    public Guid BoardId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Board Board { get; set; } = null!;
}
