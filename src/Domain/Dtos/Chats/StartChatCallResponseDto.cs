namespace Domain.Dtos.Chats;

public class StartChatCallResponseDto
{
    public string RoomUrl { get; set; } = string.Empty;
    public ChatMessageDto Message { get; set; } = new();
}
