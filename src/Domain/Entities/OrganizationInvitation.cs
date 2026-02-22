using Domain.Enums;

namespace Domain.Entities;

public class OrganizationInvitation : AuditableEntity<Guid>
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public Guid InvitedBy { get; set; }
    public OrganizationMemberRole Role { get; set; } = OrganizationMemberRole.Guest;
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime? RespondedAt { get; set; }

    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public User User { get; set; } = null!;
    public User Inviter { get; set; } = null!;
}
