namespace Domain.Entities;

public class ChatMessage : AuditableEntity<Guid>
{
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string MessageType { get; set; } = "Text";
    public bool HasAttachments { get; set; }

    public Chat Chat { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
