using Domain.Enums;

namespace Domain.Entities;

public class ChatMember : AuditableEntity
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public ChatMemberRole Role { get; set; } = ChatMemberRole.Member;
    public DateTime? LastReadAt { get; set; }

    public Chat Chat { get; set; } = null!;
    public User User { get; set; } = null!;
}
