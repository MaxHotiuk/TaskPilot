using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Meeting : AuditableEntity<Guid>
{
    public Guid BoardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Description { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public int? Duration { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    public Guid CreatedBy { get; set; }

    // Navigation properties
    public Board Board { get; set; } = null!;
    public User Creator { get; set; } = null!;
    public ICollection<MeetingMember> Members { get; set; } = new List<MeetingMember>();
}
