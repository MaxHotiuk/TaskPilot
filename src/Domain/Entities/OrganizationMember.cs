using Domain.Enums;

namespace Domain.Entities;

public class OrganizationMember : AuditableEntity
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public bool IsInvited { get; set; } = false;
    public OrganizationMemberRole Role { get; set; } = OrganizationMemberRole.Member;

    // Navigation properties
    public Organization Organization { get; set; } = null!;
    public User User { get; set; } = null!;
}
