namespace Domain.Dtos.Chat;

public class ChatRequest
{
    public string? Message { get; set; }
    public string? SessionId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? UserId { get; set; }
}
