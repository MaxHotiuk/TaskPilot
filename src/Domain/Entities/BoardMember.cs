namespace Domain.Entities;

public class BoardMember
{
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = "Member";
    public DateTime JoinedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public Board Board { get; set; } = null!;
    public User User { get; set; } = null!;
}
