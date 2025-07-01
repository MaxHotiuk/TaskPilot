namespace Domain.Entities;

public class User : IEntity<Guid>, IAuditableEntity
{
    public Guid Id { get; set; }
    public string EntraId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Board> OwnedBoards { get; set; } = new List<Board>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<BoardMember> BoardMemberships { get; set; } = new List<BoardMember>();
}
