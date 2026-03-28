using Domain.Enums;

namespace Domain.Dtos.Chats;

public class ChatMemberDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ChatMemberRole Role { get; set; }
    public DateTime? LastReadAt { get; set; }
}
