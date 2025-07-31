using System;

namespace Domain.Entities;

public class MeetingMember : AuditableEntity
{
    public Guid MeetingId { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = "Invited"; // Invited, Accepted, Declined, Tentative, Attended

    // Navigation properties
    public Meeting Meeting { get; set; } = null!;
    public User User { get; set; } = null!;
}
