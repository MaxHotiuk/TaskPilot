using Domain.Enums;

namespace Domain.Entities;

public class Notification : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public Guid? BoardId { get; set; }
    public Guid? TaskId { get; set; }
    public bool IsRead { get; set; } = false;

    // Navigation properties
    public User User { get; set; } = null!;
    public Board? Board { get; set; }
    public TaskItem? Task { get; set; }
}
