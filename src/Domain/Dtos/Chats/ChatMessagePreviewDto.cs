namespace Domain.Dtos.Chats;

public class ChatMessagePreviewDto
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string MessageType { get; set; } = "Text";
    public DateTime CreatedAt { get; set; }
}
