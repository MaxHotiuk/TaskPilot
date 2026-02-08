using Domain.Enums;

namespace Domain.Entities;

public class Chat : AuditableEntity<Guid>
{
    public Guid OrganizationId { get; set; }
    public Guid? BoardId { get; set; }
    public string? Name { get; set; }
    public ChatType Type { get; set; }
    public Guid CreatedById { get; set; }

    public Organization Organization { get; set; } = null!;
    public Board? Board { get; set; }
    public User CreatedBy { get; set; } = null!;
    public ICollection<ChatMember> Members { get; set; } = new List<ChatMember>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
