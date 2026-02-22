using Domain.Enums;

namespace Domain.Entities;

public class BoardInvitation : AuditableEntity<Guid>
{
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public Guid InvitedBy { get; set; }
    public string Role { get; set; } = "Member";
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime? RespondedAt { get; set; }

    // Navigation properties
    public Board Board { get; set; } = null!;
    public User User { get; set; } = null!;
    public User Inviter { get; set; } = null!;
}
