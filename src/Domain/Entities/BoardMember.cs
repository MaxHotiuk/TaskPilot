namespace Domain.Entities;

public class BoardMember : AuditableEntity
{
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = "Member";

    // Navigation properties
    public Board Board { get; set; } = null!;
    public User User { get; set; } = null!;
}
