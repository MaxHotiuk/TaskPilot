using Domain.Enums;

namespace Domain.Entities;

public class ArchivalJob : AuditableEntity<Guid>
{
    public Guid BoardId { get; set; }
    public string JobType { get; set; } = "BoardArchival";
    public ArchivalStatus Status { get; set; } = ArchivalStatus.Pending;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? BlobPath { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ProcessedBy { get; set; }
    public string? Metadata { get; set; }

    // Navigation properties
    public Board Board { get; set; } = null!;
}