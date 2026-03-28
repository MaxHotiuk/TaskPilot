using Domain.Enums;

namespace Domain.Dtos.Chats;

public class ChatDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? BoardId { get; set; }
    public string? Name { get; set; }
    public ChatType Type { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ChatMessagePreviewDto? LastMessage { get; set; }
    public IEnumerable<ChatMemberDto> Members { get; set; } = new List<ChatMemberDto>();
}
