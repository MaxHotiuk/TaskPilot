namespace Domain.Dtos.Chats;

public class SendChatMessageRequestDto
{
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
}
