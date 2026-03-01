using Domain.Enums;

namespace Domain.Entities;

public class OrganizationManagerRequest : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public string Message { get; set; } = string.Empty;
    public ManagerRequestStatus Status { get; set; } = ManagerRequestStatus.Pending;
    public Guid? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
    public User? Reviewer { get; set; }
}
