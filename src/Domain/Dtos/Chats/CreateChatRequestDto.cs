using Domain.Enums;

namespace Domain.Dtos.Chats;

public class CreateChatRequestDto
{
    public Guid OrganizationId { get; set; }
    public Guid CreatedById { get; set; }
    public ChatType Type { get; set; }
    public string? Name { get; set; }
    public IEnumerable<Guid> MemberIds { get; set; } = new List<Guid>();
}
