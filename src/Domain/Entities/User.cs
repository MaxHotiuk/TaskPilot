namespace Domain.Entities;

public class User : AuditableEntity<Guid>
{
    public string EntraId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    // Google Calendar integration
    public string? GoogleAccessToken { get; set; }
    public string? GoogleRefreshToken { get; set; }
    public DateTime? GoogleTokenExpiry { get; set; }
    public bool IsGoogleCalendarConnected { get; set; } = false;

    // Navigation properties
    public ICollection<Board> OwnedBoards { get; set; } = new List<Board>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<BoardMember> BoardMemberships { get; set; } = new List<BoardMember>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<OrganizationMember> OrganizationMemberships { get; set; } = new List<OrganizationMember>();
    public ICollection<Chat> CreatedChats { get; set; } = new List<Chat>();
    public ICollection<ChatMember> ChatMemberships { get; set; } = new List<ChatMember>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
}
